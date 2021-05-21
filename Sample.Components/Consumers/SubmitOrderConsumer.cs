using Sample.Contracts;
using MassTransit;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Sample.Components.Consumers
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        ILogger<SubmitOrderConsumer> _logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger.Log(LogLevel.Debug, $"SubmitOrderConsumer {context.Message.CustomerNumber}");

            if (context.Message.CustomerNumber.ToUpper().Contains("TEST"))
            {
                if (context.RequestId != null)
                {
                    await context.RespondAsync<OrderSubmissionRejected>((
                        InVar.Timestamp,
                        context.Message.OrderId,
                        context.Message.CustomerNumber,
                        Reason: $"Test customers cannot submit orders: {context.Message.CustomerNumber}"
                    ));
                }
                return;
            }

            await context.Publish<OrderSubmitted>(
                new
                {
                    context.Message.OrderId,
                    context.Message.CustomerNumber,
                    context.Message.Timestamp
                });

            if (context.RequestId != null)
            {
                await context.RespondAsync<OrderSubmissionAccepted>(new
                {
                    Timestamp = InVar.Timestamp,
                    OrderId = context.Message.OrderId,
                    CustomerNumber = context.Message.CustomerNumber,
                });
            }
        }
    }
}