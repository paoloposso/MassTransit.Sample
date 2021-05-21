using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Contracts;
using System;
using System.Threading.Tasks;

namespace Sample.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<CheckOrder> _checkOrderClient;

        public OrderController(ILogger<OrderController> logger, IRequestClient<SubmitOrder> submitOrderRequestClient, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpoint publishEndpoint, IRequestClient<CheckOrder> checkOrderClient)
        {
            _submitOrderRequestClient = submitOrderRequestClient;
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _checkOrderClient = checkOrderClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var (status, notFound) = await _checkOrderClient.GetResponse<OrderStatus, OrderNotFound>(new
            {
                OrderId = id
            });

            if (status.IsCompletedSuccessfully)
            {
                var res = await status;
                return Ok(res.Message);
            }

            var notFoundRes = await notFound;
            return NotFound(notFoundRes.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid id, string customerNumber)
        {
            var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
            {
                OrderId = id,
                CustomerNumber = customerNumber,
                Timestamp = InVar.Timestamp
            });

            if (accepted.IsCompletedSuccessfully)
            {
                return Accepted(await accepted);
            }

            return BadRequest(await rejected);
        }

        [HttpPut]
        public async Task Put(Guid id, string customerNumber)
        {
            _logger.LogInformation("Put");

            await _publishEndpoint.Publish<SubmitOrder>(new
            {
                OrderId = id,
                CustomerNumber = customerNumber,
                Timestamp = InVar.Timestamp
            });
        }
    }
}