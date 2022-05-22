using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.WebApi.Order.Extensions;
using Microsoft.OpenApi.Models;
using RabbitMQ.Services;
using Infrastructure.Producer;

namespace RabbitMQ.WebApi.Order
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c=>
            {
                c.SwaggerDoc("v1",new OpenApiInfo { Title="RabbitMQ.WebApi.Order",Version="v1"});
            });
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IRabbitProducer,RabbitProducer>();
            services.AddSingleton<IHostedService, ProcessPay>();
            services.AddSingleton<IHostedService, ProcessPayTimeout>();
            services.AddSingleton<IPayService, PayService>();
            services.ConfigureCors();
            services.ConfigureRabbitContext(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c=>c.SwaggerEndpoint("/swagger/v1/swagger.json","RabbitMQ.WebApi.Order v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
