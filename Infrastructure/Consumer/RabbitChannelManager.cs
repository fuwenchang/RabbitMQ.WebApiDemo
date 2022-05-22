using Infrastructure.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Consumer
{
    public class RabbitChannelManager
    {
        public RabbitConnection Connection { get; set; }

        public RabbitChannelManager(RabbitConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// 创建接收消息的通道
        /// </summary>
        /// <param name="exchangeType"></param>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="routingKey"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public RabbitChannelConfig CreateReceiveChannel(string exchangeType,string exchange,string queue,string routingKey,
            IDictionary<string,object>arguments = null)
        {
            IModel model = this.CreateModel(exchangeType,exchange,queue,routingKey,arguments);
            EventingBasicConsumer consumer = this.CreateConsumer(model,queue);
            RabbitChannelConfig channel = new RabbitChannelConfig(exchangeType,exchange,queue,routingKey);
            // 使用事件的方式消费
            consumer.Received += channel.Receive;
            return channel;
        }

      
        /// <summary>
        /// 创建一个通道，包含交换机/队列/路由，并建立绑定关系
        /// </summary>
        /// <param name="exchangeType">交换机类型：Topic,Direct,Fanout</param>
        /// <param name="exchange">交换机名称</param>
        /// <param name="queue">队列名称</param>
        /// <param name="routingKey">路由名称</param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private IModel CreateModel(string exchangeType, string exchange, string queue, string routingKey, IDictionary<string, object> arguments)
        {
            exchangeType = string.IsNullOrEmpty(exchangeType) ? "default" : exchangeType;
            IModel model = this.Connection.GetConnection().CreateModel();
            model.BasicQos(0,1,false);
            model.QueueDeclare(queue,true,false,false,arguments);
            model.ExchangeDeclare(exchange, exchangeType);
            model.QueueBind(queue, exchange, routingKey);
            return model;
        }

        /// <summary>
        /// 创建消费者
        /// </summary>
        /// <param name="model"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
        /// <remarks>
        /// 消费消息有两种模式
        /// 1:主动模式
        ///     主动从队列中去拉取消息。
        ///     优点：消费灵活，可以随时消费
        /// 2:被动模式        
        ///     一旦有消息进来，马上触发消费
        ///     优点：即时性好
        ///     
        /// 本示例是采用被动模式（事件）
        /// </remarks>
        private EventingBasicConsumer CreateConsumer(IModel model, string queue)
        {            
            EventingBasicConsumer consumer = new EventingBasicConsumer(model);
            //使用事件的方式(队列名称，是否立马应答，绑定事件)(如果不想消息被重复读取将 false改为true)
            model.BasicConsume(queue, false, consumer);
            return consumer;
        }
    }
}
