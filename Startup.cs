using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroserviceConsole
{
    public class Startup
    {
        private string _connectionString;
        private string _serviceBusName;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _connectionString = Configuration["connectionString"];
            _serviceBusName = Configuration["ServiceBus"];
            services.AddHealthEndpoints();
            services.AddSingleton<IQueueClient>(x => new QueueClient(
                    _connectionString,
                    _serviceBusName));
            services.AddHostedService<ServiceBusConsumer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Run(async (context) =>
            {
                var connectionStingresult = string.IsNullOrEmpty(_connectionString) ? "Null" : "Not Null";
                var serviceBusNameResult = string.IsNullOrEmpty(_serviceBusName) ? "Null" : "Not Null";
                await context.Response.WriteAsync($"Secret is {connectionStingresult} and {serviceBusNameResult}");
            });
            app.UseHealthEndpoint();
        }
    }
}