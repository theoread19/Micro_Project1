using Autofac;
using Autofac.Extensions.DependencyInjection;
using Confluent.Kafka;
using Domain.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ProjectCore.Logging;
using ProjectCore.Startup;
using UserProject.Services;
using UserProject.Services.iplm;
using WebApiContrib.Core.Formatter.Protobuf;

namespace UserProject
{
    public class Startup : BaseStartup
    {
        public IContainer ApplicationContainer { get; private set; } = null!;

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddControllers()
                .AddProtobufFormatters();
            
            //api config

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", option =>
                {
                    
                    option.Authority = "https://localhost:44305";
                    option.RequireHttpsMetadata = false;
                    option.ApiSecret = "Secret";


                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("myPolicy", builder =>
                {
                   //require api scope 
                    builder.RequireScope("api1");
                });
            });
            //end api config

            //config kafka producer
            var producerConfig = new ProducerConfig();
            Configuration.Bind("producer", producerConfig);

            services.AddSingleton<ProducerConfig>(producerConfig);

            
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TheCodeBuzz-Service", Version = "v1" });
                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });

                //////Add Operation Specific Authorization///////
                c.OperationFilter<AuthOperationFilter>();
                ////////////////////////////////////////////////

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

//            services.AddSingleton<ILoggerManager, LoggerManager>();

            builder.RegisterType<UserRepository>().As<IUserService>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<MessageService>().As<IMessageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
        {

            base.Configure(app, env, logger);
            /*if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.ConfigureExceptionHandler((LoggerManager)logger);

            app.UseRouting();

            app.UseAuthorization();*/

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
            });
        }

    }
}
