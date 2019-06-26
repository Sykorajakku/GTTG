using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;

using SZDC.Data.Model;

namespace SZDC.Data {

    public class SzdcContext : DbContext {

        public DbSet<Railway> Railways { get; set; }
        public DbSet<Train> Trains { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

            optionsBuilder
                .UseNpgsql(new NpgsqlConnection(ConfigurationManager.AppSettings["Connection string"]));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder
                .Entity<Train>()
                .HasMany<RailwaySection>();
        }
    }
}
