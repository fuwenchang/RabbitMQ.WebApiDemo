using Infrastructure.Config;
using Infrastructure.Producer;
using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Entities;
using Newtonsoft.Json;
using Infrastructure.Message;

namespace RabbitMQ.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRabbitProducer _rabbitProducer;
        public OrderService(IRabbitProducer rabbitProducer)
        {
            _rabbitProducer = rabbitProducer;
        }
        public void SendOrderMessage()
        {
            string message = JsonConvert.SerializeObject(GetOrderMessage());
            Console.WriteLine("短信/邮件异步通知");
            Console.WriteLine($"send message:{message}");
            //支付服务
            _rabbitProducer.Publish(RabbitConstant.DELAY_EXCHANGE, RabbitConstant.DELAY_ROUTING_KEY,
                new Dictionary<string, object>()
                {
                    { "x-delay",1000*2}
                },message);
        }

        private OrderMessage GetOrderMessage() 
        {
            OrderInfo orderInfo = new OrderInfo();
            orderInfo.GoodsCount = 1;
            orderInfo.GoodsId = 1;
            orderInfo.GoodsName = "大话设计模式";
            orderInfo.Status = 0;
            orderInfo.UserId = 1;
            Account account = new Account();
            account.UserName = "Hobelee";
            account.Password = "password007";
            account.Email = "hobelee@163.com";
            account.Phone = "13964836342";

            OrderMessage orderMessage = new OrderMessage();
            orderMessage.Account = account;
            orderMessage.OrderInfo = orderInfo;

            return orderMessage;
        }

        public void SendTestMessage(string message)
        {
            Console.WriteLine($"send message:{message}");
            _rabbitProducer.Publish(RabbitConstant.TEST_EXCHANGE,"",new Dictionary<string,object>(),message);
        }
    }
}
