using System;

namespace Sample.Contracts
{
    public class SubmitOrder
    {
         public Guid OrderId { get; set; }
         public DateTime Timestamp { get; set; }
         public string CustomerNumber { get; set; }
    }

    public class OrderSubmissionAccepted
    {
        public Guid OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string CustomerNumber { get; set; }
    }

    public class OrderSubmissionRejected
    {
        public Guid OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string CustomerNumber { get; set; }
        string Reason { get; set; }
    }
}