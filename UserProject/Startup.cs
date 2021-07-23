using Autofac;
using Autofac.Extensions.DependencyInjection;
using Confluent.Kafka;
using Domain.Logging;
using Domain.Repository;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Infrastructure.IdentityConfigure;
using Infrastructure.Logging;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UserProject.CustomerExceptionMiddleware;
using UserProject.Services;
using UserProject.Services.iplm;
using WebApiContrib.Core.Formatter.Protobuf;

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
            services.AddControllers()
                .AddProtobufFormatters();

            //api config
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", option =>
                {
                    
                    option.Authority = "https://localhost:44305";
                    option.RequireHttpsMetadata = false;
                    option.ApiSecret = "Secret";
                    option.ApiName = "api1";

                });

            services.AddAuthorization(options => options.AddPolicy("Admin", policy => policy.RequireClaim("role", "Admin")));
            services.AddAuthorization(options => options.AddPolicy("Member", policy => policy.RequireClaim("role", "Member")));
            //end api config

            //config kafka producer
            var producerConfig = new ProducerConfig();
            Configuration.Bind("producer", producerConfig);

            services.AddSingleton<ProducerConfig>(producerConfig);

            
            
            services.AddSwaggerGen(options => {

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,

                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:44305/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:44305/connect/token"),
                            Scopes = new Dictionary<string, string>
                           {
                               {"api1" , "My Api"},
                               {"roles", "User Roles" }
                           }
                        }
                    }

                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });


            });

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

//            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.ConfigureExceptionHandler((LoggerManager)logger);

            app.UseRouting();


            app.UseAuthentication();
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
                c.OAuthClientId("ro.client");
                c.OAuthClientSecret("Secret");
            });
        }

    }
}
