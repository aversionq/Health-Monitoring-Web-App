using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using HealthMonitoringApp.Application.Interfaces;
using HealthMonitoringApp.Core.Entities;
using System.Reflection.Emit;

namespace HealthMonitoringApp.Data.DatabaseContext
{
    public partial class HealthMonitoringDbContext : DbContext
    {
        public HealthMonitoringDbContext()
        {
        }

        public HealthMonitoringDbContext(DbContextOptions<HealthMonitoringDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Pressure> Pressures { get; set; } = null!;
        public virtual DbSet<HeartRate> HeartRates { get; set; } = null!;
        public virtual DbSet<BloodSugar> BloodSugars { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pressure>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<HeartRate>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<BloodSugar>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}