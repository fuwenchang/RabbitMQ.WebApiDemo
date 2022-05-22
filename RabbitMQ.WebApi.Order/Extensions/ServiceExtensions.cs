using Infrastructure.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.WebApi.Order.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options=>
            {
                options.AddPolicy("AnyPolicy",
                    builder=>builder.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });
        }
        public static void ConfigureRabbitContext(this IServiceCollection services,IConfiguration config)
        {
            var section = config.GetSection("RabbitMQ");
            services.AddSingleton(
                  new RabbitConnection(section.Get<RabbitOption>())); 
        }
    }
}
