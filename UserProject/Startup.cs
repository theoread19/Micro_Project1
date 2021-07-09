using Autofac;
using Autofac.Extensions.DependencyInjection;
using Domain.Logging;
using Domain.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserProject.Services;
using UserProject.Services.iplm;
using Infrastructure.Logging;
using Domain.Models;
using Confluent.Kafka;
using WebApiContrib.Core.Formatter.Protobuf;
using UserProject.CustomerExceptionMiddleware;

namespace UserProject
{
    public class Startup
    {


        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; } = null!;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //config kafka producer
            var producerConfig = new ProducerConfig();
            Configuration.Bind("producer", producerConfig);

            services.AddSingleton<ProducerConfig>(producerConfig);

            services.AddControllers()
                .AddProtobufFormatters();
            
            services.AddSwaggerGen();

            var builder = new ContainerBuilder();
            builder.Populate(services);

            this.ApplicationContainer = builder.Build();
            //repository
            services.AddScoped<IUserRepository, UserRepository>();

            //service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMessageService, MessageService>();
            //

            services.AddSingleton<ILoggerManager, LoggerManager>();

            builder.RegisterType<UserRepository>().As<IUserService>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<MessageService>().As<IMessageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.ConfigureExceptionHandler((LoggerManager)logger);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),  
            // specifying the Swagger JSON endpoint.  
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
