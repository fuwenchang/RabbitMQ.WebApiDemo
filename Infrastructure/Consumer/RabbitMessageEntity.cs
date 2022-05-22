using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Consumer
{
    public class RabbitMessageEntity
    {
        public string Content { get; set; }
        public EventingBasicConsumer Consumer { get; set; }
        public BasicDeliverEventArgs BasicDeliver { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
        public bool Error { get; set; }
        public int Code { get; set; }
    }
}
