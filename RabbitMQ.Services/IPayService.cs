using RabbitMQ.Entities;

namespace RabbitMQ.Services
{
    public interface IPayService
    {
        void UpdateOrderPayState(OrderInfo orderInfo);
    }
}
