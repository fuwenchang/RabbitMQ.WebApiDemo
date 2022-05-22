using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Consumer
{
    public class QueueInfo
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        /// 路由名称
        /// </summary>
        public string RoutingKey { get; set; }
        /// <summary>
        /// 交换机类型
        /// </summary>
        public string ExchangeType { get; set; }
        /// <summary>
        /// 交换机名称
        /// </summary>
        public string Exchange { get; set; }
        public IDictionary<string, object> props { get; set; } = null;
        public Action<RabbitMessageEntity> OnRecevied { get; set; }
    }
}
