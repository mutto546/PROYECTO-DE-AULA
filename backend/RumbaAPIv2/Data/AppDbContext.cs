using Microsoft.EntityFrameworkCore;
using RumbaAPI.Models;

namespace RumbaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User>    Users    { get; set; }
        public DbSet<Rumba>   Rumbas   { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Username y Email únicos
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Un usuario tiene muchas rumbas — si se borra el user, se borran sus rumbas
            modelBuilder.Entity<Rumba>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rumbas)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Un usuario tiene muchos clientes
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.User)
                .WithMany(u => u.Clientes)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Una rumba puede tener un cliente (opcional)
            // Si se borra el cliente, la rumba queda sin cliente (SetNull)
            modelBuilder.Entity<Rumba>()
                .HasOne(r => r.Cliente)
                .WithMany(c => c.Rumbas)
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
