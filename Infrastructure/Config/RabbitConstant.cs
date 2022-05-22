using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Config
{
    public class RabbitConstant
    {
        public const string TEST_EXCHANGE = "test.exchange";
        public const string TEST_QUEUE = "test.queue";

        public const string DELAY_EXCHANGE = "delay.exchange";
        public const string DELAY_ROUTING_KEY = "delay.routing.key";
        public const string DELAY_QUEUE = "delay.queue";

        public const string DEAD_LETTER_EXCHANGE = "dead.letter.exchange";
        public const string DEAD_LETTER_QUEUE = "dead.letter.queue";
        public const string DEAD_LETTER_ROUTING_KEY = "dead.letter.routing.key";
    }


    public class RabbitQueueConfig 
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public string Queue { get; set; }
    }
}
