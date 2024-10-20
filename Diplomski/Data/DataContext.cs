using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Diplomski.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }

        public DbSet<PermissionSet> PermissionSets { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<PermissionSet>()
                .HasKey(ps => ps.Id);

            
            modelBuilder.Entity<PermissionSet>()
                .HasIndex(ps => ps.Name)
                .IsUnique();

            modelBuilder.Entity<Permission>()
                .HasKey(p => p.id);

            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
