using System;
using System.Collections.Generic;

namespace Infrastructure.Producer
{
    public interface IRabbitProducer
    {
        public void Publish(string exchange,string routingKey,IDictionary<string,object> props,string content);
    }
}
