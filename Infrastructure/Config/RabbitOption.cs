using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Config
{
    public class RabbitOption
    {
        /// <summary>
        /// 主机名称
        /// </summary>
        public string HostName { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string VirtualHost { get; set; }
    }
}
