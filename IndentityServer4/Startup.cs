using Domain.Models;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IndentityServer4
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

            var connectionstring = Configuration.GetConnectionString("DefaultConnection");
            var migrationAssemble = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //steps for get user from exit Database for Identityserver4
            /*          Make a user class containing the relevant fields as your database
             *          Create an EntityFramework DbContext class to map your database to your class
             *          Register your user class and dbcontext with aspnet core identity
             *          Tell IdentityServer to use AspNetIdentity*/

/*
            services.AddEntityFrameworkSqlServer()
                   .AddDbContext<AppDbContext>(options =>
                       options.UseSqlServer(connectionstring));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();*/




            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
//                .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
//                .AddInMemoryClients(IdentityConfig.Clients)
                .AddTestUsers(IdentityConfig.GetUsers())
//                .AddInMemoryIdentityResources(IdentityConfig.GetIdentityResourceResources())
                .AddProfileService<ProfileService>() // load information for us according to the claim type required by the request.
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>() // define our own verification logic.
               .AddConfigurationStore(option =>
               {
                   option.ConfigureDbContext = builder => builder.UseSqlServer(connectionstring, opt => opt.MigrationsAssembly(migrationAssemble));
               })
               .AddOperationalStore(option =>
               {
                   option.ConfigureDbContext = builder => builder.UseSqlServer(connectionstring, opt => opt.MigrationsAssembly(migrationAssemble));
               })
               .AddDeveloperSigningCredential();
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

//            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
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
                    foreach (var resource in IdentityConfig.GetIdentityResourceResources())
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
        }
    }
}
