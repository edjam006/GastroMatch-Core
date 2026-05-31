using Microsoft.EntityFrameworkCore;
using GastroMatch_Core.Models;

namespace GastroMatch_Core.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<PreferenciaUsuario> PreferenciasUsuario { get; set; } = null!;
        public DbSet<Restaurante> Restaurantes { get; set; } = null!;
        public DbSet<Plato> Platos { get; set; } = null!;
        public DbSet<Ingrediente> Ingredientes { get; set; } = null!;
        public DbSet<Pedido> Pedidos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Relación 1-a-1: Usuario <-> PreferenciaUsuario
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Preferencia)
                .WithOne(p => p.Usuario)
                .HasForeignKey<PreferenciaUsuario>(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. Relación 1-a-N: Usuario -> Pedido
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Pedidos)
                .WithOne(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. Relación 1-a-N: Restaurante -> Plato
            modelBuilder.Entity<Restaurante>()
                .HasMany(r => r.Platos)
                .WithOne(p => p.Restaurante)
                .HasForeignKey(p => p.RestauranteId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4. Relación Muchos-a-Muchos implícita: Pedido <-> Plato
            modelBuilder.Entity<Pedido>()
                .HasMany(p => p.Platos)
                .WithMany(p => p.Pedidos)
                .UsingEntity(j => j.ToTable("PedidoPlato"));

            // 5. Relación Muchos-a-Muchos implícita: Plato <-> Ingrediente
            modelBuilder.Entity<Plato>()
                .HasMany(p => p.Ingredientes)
                .WithMany(i => i.Platos)
                .UsingEntity(j => j.ToTable("PlatoIngrediente"));

            // Configurar precisión decimal para precios
            modelBuilder.Entity<PreferenciaUsuario>()
                .Property(p => p.RangoPrecioMax)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Plato>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.TotalPagar)
                .HasPrecision(18, 2);

            // Precisiones de Latitud y Longitud (10, 6)
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Latitud)
                .HasPrecision(10, 6);
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Longitud)
                .HasPrecision(10, 6);

            modelBuilder.Entity<Restaurante>()
                .Property(r => r.Latitud)
                .HasPrecision(10, 6);
            modelBuilder.Entity<Restaurante>()
                .Property(r => r.Longitud)
                .HasPrecision(10, 6);

            // Configurar el Enum de Pedido como string en la base de datos
            modelBuilder.Entity<Pedido>()
                .Property(p => p.Estado)
                .HasConversion<string>();
        }
    }
}
