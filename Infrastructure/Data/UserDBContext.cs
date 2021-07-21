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


        public virtual DbSet<UserModel> UserTable { get; set; } = null!;

        public UserDBContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory!;
                string relativePath = @"..\..\..\..\UserProject";
                string webPath = Path.GetFullPath(basePath + relativePath);
                IConfigurationRoot configuration = new ConfigurationBuilder()
                                                .SetBasePath(webPath)
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

                e.HasIndex(e => e.Username, "UQ_username")
                    .IsUnique();

                e.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("fullname");

                e.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                e.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(50);

                e.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("passsword")
                    .HasMaxLength(50);

                e.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("role");
            });

           
        }
        
    }
}
