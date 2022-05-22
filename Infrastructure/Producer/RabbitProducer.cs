using Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Producer
{
    public class RabbitProducer : IRabbitProducer
    {
        private readonly RabbitConnection _connection;
        public RabbitProducer(RabbitConnection connection)
        {
            _connection = connection;
        }
        public void Publish(string exchange, string routingKey, IDictionary<string, object> props, string content)
        {
            var channel = _connection.GetConnection().CreateModel();
     
            var prop = channel.CreateBasicProperties();
            if (props.Count > 0)
            {
                var delay = props["x-delay"];
                prop.Expiration = delay.ToString();
            }
            channel.BasicPublish(exchange, routingKey, false, prop, Encoding.UTF8.GetBytes(content));
        }
    }
}
