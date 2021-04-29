using Automatonymous;
using MassTransit.RedisIntegration;
using MassTransit.Saga;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.StateMachines
{
    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));

            InstanceState(x => x.CurrentState);

            Initially(
                When(OrderSubmitted)
                .Then(context =>
                {
                    context.Instance.CustomerNumber = context.Data.CustomerNumber;
                    context.Instance.Updated = DateTime.UtcNow;
                    context.Instance.SubmitDate = DateTime.UtcNow;
                })
                .TransitionTo(Submitted));

            During(Submitted, Ignore(OrderSubmitted));

            DuringAny(When(OrderSubmitted)
                .Then(context =>
                {
                    context.Instance.CustomerNumber = context.Data.CustomerNumber;
                    context.Instance.SubmitDate = context.Data.Timestamp;
                })
            );
        }

        public State Submitted { get; private set; }
        public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    }

    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string CustomerNumber { get; set; }
        public int Version { get; set; }
        public DateTime Updated { get; set; }
        public DateTime SubmitDate { get; set; }
    }
}
