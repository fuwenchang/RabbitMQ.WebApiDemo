using Infrastructure.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace RabbitMQ.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var section = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("RabbitMQ");
            var host = new HostBuilder()
                .ConfigureServices(services =>
                 services.AddSingleton(new RabbitConnection(section.Get<RabbitOption>()))
                         .AddSingleton<IHostedService,ProcessTest>())
                .Build();
             host.Run();
        }
    }
}
