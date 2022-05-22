using Infrastructure.Config;
using Infrastructure.Consumer;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Test
{
    public class ProcessTest : IHostedService
    {
        private readonly RabbitConnection _connection;
        public List<QueueInfo> Queues { get; } = new List<QueueInfo>();
        public ProcessTest(RabbitConnection connection)
        {
            _connection = connection;
            Queues.Add(new QueueInfo()
            { 
               ExchangeType = ExchangeType.Fanout,
               Exchange = RabbitConstant.TEST_EXCHANGE,
               Queue = RabbitConstant.TEST_QUEUE,
               RoutingKey = "",
               OnRecevied = this.Receive
            });
        }

        private void Receive(RabbitMessageEntity message)
        {
            Console.WriteLine($"Test Receive Message:{message.Content}");
            message.Consumer.Model.BasicAck(message.BasicDeliver.DeliveryTag,true);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RabbitMQ测试消息接收处理服务正在启动");
            RabbitChannelManager channelManager = new RabbitChannelManager(_connection);
            foreach (var queueInfo in Queues)
            {
                RabbitChannelConfig channel = channelManager.CreateReceiveChannel(queueInfo.ExchangeType,
                    queueInfo.Exchange,queueInfo.Queue,queueInfo.RoutingKey);
                channel.OnReceivedCallback = queueInfo.OnRecevied;
            }
            Console.WriteLine("RabbitMQ测试消息接收处理服务已经启动");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
