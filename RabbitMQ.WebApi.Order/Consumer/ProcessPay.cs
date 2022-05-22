using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Config;
using Infrastructure.Consumer;
using Infrastructure.Message;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Entities;
using RabbitMQ.Services;

namespace RabbitMQ.WebApi.Order
{
    /// <summary>
    /// 消费支付提醒
    /// </summary>
    /// <remarks>
    /// IHostedService
    ///     即可以将 后台任务 作为托管服务的模式，程序启动后，会执行下面的方法 StartAsync 
    ///     
    /// 要注入，在StartUp.cs中
    /// </remarks>
    [Obsolete("可不用消费待过期的队列，一定时间后，如果是延迟队列，会进入死信队列，直接消费死信队列（如十分钟后，看用户有没有上传过新的产品）")]
    public class ProcessPay : IHostedService
    {
        private readonly RabbitConnection _connection;
        private readonly IPayService _payService;
        public List<RabbitChannelConfig> Channels { get; set; }
        public List<QueueInfo> Queues { get; } = new List<QueueInfo>();
        public ProcessPay(RabbitConnection connection, IPayService payService)
        {
            _connection = connection;
            _payService = payService;
            Queues.Add(new QueueInfo()
            { 
               ExchangeType = ExchangeType.Direct,
               Exchange = RabbitConstant.DELAY_EXCHANGE,
               Queue = RabbitConstant.DELAY_QUEUE,
               RoutingKey = RabbitConstant.DELAY_ROUTING_KEY,
               props = new Dictionary<string, object>()
               {
                   { "x-dead-letter-exchange",RabbitConstant.DEAD_LETTER_EXCHANGE},
                   { "x-dead-letter-routing-key",RabbitConstant.DEAD_LETTER_ROUTING_KEY}
               },
               OnRecevied = this.Receive
            });
        }

        private void Receive(RabbitMessageEntity message)
        {
            Console.WriteLine($"Pay Receive Message:{message.Content}");
            OrderMessage orderMessage = JsonConvert.DeserializeObject<OrderMessage>(message.Content);

            //超时未支付
            string money = "";
            //支付处理
            Console.WriteLine("请输入:");
            //超时未支付进行处理
            //Task.Run(()=>
            //{
            //    money = Console.ReadLine();
            //}).Wait(20*1000);
            if (string.Equals(money, "100"))
            {
                //设置状态为支付成功(同时设置消息的状态和数据库订单的状态)
                orderMessage.OrderInfo.Status = 1;
                _payService.UpdateOrderPayState(orderMessage.OrderInfo);
                Console.WriteLine("支付完成");
                message.Consumer.Model.BasicAck(deliveryTag:message.BasicDeliver.DeliveryTag,multiple:true);
            }
            else
            {
                //重试几次依然失败
                Console.WriteLine("等待一定时间失效超时未支付的订单");
                //消息进入到死信队列（可以注释掉，死信队列是超时，或者拒绝的都可以进入到死信队列）
                message.Consumer.Model.BasicNack(deliveryTag: message.BasicDeliver.DeliveryTag,
                                                  multiple: false,
                                                  requeue: false);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RabbitMQ支付通知处理服务已启动");
            RabbitChannelManager channelManager = new RabbitChannelManager(_connection);
            foreach (var queueInfo in Queues)
            {
                RabbitChannelConfig channel = channelManager.CreateReceiveChannel(queueInfo.ExchangeType,
                    queueInfo.Exchange,queueInfo.Queue,queueInfo.RoutingKey,queueInfo.props);
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
