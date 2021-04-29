﻿using System;

namespace Sample.Contracts
{
    public class OrderSubmissionAccepted
    {
        public Guid OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string CustomerNumber { get; set; }
    }
}