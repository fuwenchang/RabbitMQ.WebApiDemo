using RabbitMQ.Entities;
using System;

namespace RabbitMQ.Services
{
    public class PayService : IPayService
    {
        public void UpdateOrderPayState(OrderInfo orderInfo)
        {
            Console.WriteLine($"修改订单状态:{orderInfo.Status}");
        }
    }
}
