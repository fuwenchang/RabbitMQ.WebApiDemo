using RabbitMQ.Entities;

namespace Infrastructure.Message
{
    public class OrderMessage
    {
        public Account Account { get; set; }
        public OrderInfo OrderInfo { get; set; }
    }
}
