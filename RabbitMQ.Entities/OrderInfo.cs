using System;

namespace RabbitMQ.Entities
{
    public class OrderInfo
    {
        public int GoodsCount { get; set; }
        public int GoodsId { get; set; }
        public string GoodsName { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
    }
}
