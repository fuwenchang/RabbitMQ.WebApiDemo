using System;

namespace RabbitMQ.Services
{
    public interface IOrderService
    {
        void SendOrderMessage();
        void SendTestMessage(string message);
    }
}
