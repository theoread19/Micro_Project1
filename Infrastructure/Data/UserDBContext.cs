using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Infrastructure.Data
{
    public partial class UserDBContext : DbContext
    {

        public UserDBContext()
        {
        }


        public virtual DbSet<UserModel> UserTable { get; set; }

        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                IConfigurationRoot configuration = new ConfigurationBuilder()
                                                .SetBasePath(basePath)
                                                .AddJsonFile("appsettings.json")
                                                .Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
               

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(e =>
            {
                e.ToTable("user-table");

                e.HasKey(e => e.Id);

                e.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("fullname");

                e.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
            });

           
        }
        
    }
}
