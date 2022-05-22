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

namespace RabbitMQ.WebApi.Order
{
    /// <summary>
    /// 应用场景
    /// 订单在十分钟之内未支付则自动取消。
    /// 新创建的店铺，如果在十天内都没有上传过商品，则自动发送消息提醒。
    /// 用户注册成功后，如果三天内没有登陆则进行短信提醒。
    /// 用户发起退款，如果三天内没有得到处理则通知相关运营人员。
    /// 预定会议后，需要在预定的时间点前十分钟通知各个与会人员参加会议。
    /// </summary>
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
