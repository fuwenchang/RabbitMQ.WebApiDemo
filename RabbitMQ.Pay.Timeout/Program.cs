using Infrastructure.Config;
using Infrastructure.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Services;
using System;

namespace RabbitMQ.Pay.Timeout
{
    class Program
    {
        static void Main(string[] args)
        {
            var configRabbit = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("RabbitMQ");
            var host = new HostBuilder()
                .ConfigureServices(services =>
                services.AddSingleton(new RabbitConnection(configRabbit.Get<RabbitOption>()))
                .AddSingleton<IHostedService, ProcessPayTimeout>()
                .AddScoped<IRabbitProducer, RabbitProducer>()
                .AddScoped<IPayService, PayService>()).Build();
            host.Run();
        }
    }
}
