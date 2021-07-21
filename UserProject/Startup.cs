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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
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
                

/*            var connectionstring = Configuration.GetConnectionString("DefaultConnection2");
            var migrationAssemble = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
*/
            //identity
/*            services.AddIdentityServer()
*//*                .AddInMemoryClients(IdentityConfig.Clients)
                .AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
                .AddInMemoryApiScopes(IdentityConfig.ApiScopes)*//*
                .AddTestUsers(IdentityConfig.GetUsers()) 
                .AddConfigurationStore(option =>
                {
                    option.ConfigureDbContext = builder => builder.UseSqlServer(connectionstring, opt => opt.MigrationsAssembly(migrationAssemble));
                })
                .AddOperationalStore(option =>
                {
                    option.ConfigureDbContext = builder => builder.UseSqlServer(connectionstring, opt => opt.MigrationsAssembly(migrationAssemble));
                })
                .AddDeveloperSigningCredential();*/


            //api config
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", option => 
                {
                    
                    option.Authority = "https://localhost:44305";
                    option.RequireHttpsMetadata = false;
                    option.ApiSecret = "secret";
                    option.ApiName = "api1";
/*                    option.EnableCaching = true;
                    option.CacheDuration = TimeSpan.FromMinutes(10);*/
                   

                });

/*
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.Authority = "http://localhost:5001";
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "mvc";
                    options.SaveTokens = true;
                });*/

/*            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", new string[] { "User", "Scope2"});
                });
            });*/

            services.AddAuthorization(options => options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin")));
            services.AddAuthorization(options => options.AddPolicy("Member", policy => policy.RequireClaim("Role", "Member")));
            //end api config

            //config kafka producer
            var producerConfig = new ProducerConfig();
            Configuration.Bind("producer", producerConfig);

            services.AddSingleton<ProducerConfig>(producerConfig);

            
            
            services.AddSwaggerGen(options => {

                options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
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


//            app.UseIdentityServer();

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

        /*private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in IdentityConfig.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in IdentityConfig.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in IdentityConfig.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }*/
    }
}
