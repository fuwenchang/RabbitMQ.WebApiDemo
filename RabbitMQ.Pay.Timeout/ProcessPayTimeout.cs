using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Config;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Services;
using Infrastructure.Consumer;
using RabbitMQ.Client;
using RabbitMQ.Entities;
using Infrastructure.Message;

namespace RabbitMQ.Pay.Timeout
{
    [Obsolete("已废弃，移动到RabbitMQ.WebApi.Order项目中")]
    public class ProcessPayTimeout : IHostedService
    {
        private readonly RabbitConnection _connection;
        private readonly IPayService _payService;
        public List<RabbitChannelConfig> Channels { get; set; } = new List<RabbitChannelConfig>();
        public List<QueueInfo> Queues { get; } = new List<QueueInfo>();
        public ProcessPayTimeout(RabbitConnection connection,IPayService payService)
        {
            _connection = connection;
            _payService = payService;
            Queues.Add(new QueueInfo
            { 
               ExchangeType = ExchangeType.Direct,
               Exchange = RabbitConstant.DEAD_LETTER_EXCHANGE,
               Queue = RabbitConstant.DEAD_LETTER_QUEUE,
               RoutingKey = RabbitConstant.DEAD_LETTER_ROUTING_KEY,
               OnRecevied = this.Receive
            });
        }

        private void Receive(RabbitMessageEntity messgae)
        {
            Console.WriteLine($"Pay Timeout Receive Message:{messgae.Content}");
            OrderMessage orderMessage = JsonConvert.DeserializeObject<OrderMessage>(messgae.Content);
            //获取到消息后，修改消息的状态为超时未支付 2
            orderMessage.OrderInfo.Status = 2;
            Console.WriteLine("超时未支付");
            _payService.UpdateOrderPayState(orderMessage.OrderInfo);
            messgae.Consumer.Model.BasicAck(messgae.BasicDeliver.DeliveryTag, true);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RabbitMQ超时支付处理程序启动");
            RabbitChannelManager channelManager = new RabbitChannelManager(_connection);
            foreach (var queueInfo in Queues)
            {
                RabbitChannelConfig channel = channelManager.CreateReceiveChannel(queueInfo.ExchangeType,
                    queueInfo.Exchange,queueInfo.Queue,queueInfo.RoutingKey);
                channel.OnReceivedCallback = queueInfo.OnRecevied;
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
