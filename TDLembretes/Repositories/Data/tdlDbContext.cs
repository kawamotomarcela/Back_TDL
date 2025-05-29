using Microsoft.EntityFrameworkCore;
using TDLembretes.Models;

namespace TDLembretes.Repositories.Data
{
    public partial class tdlDbContext : DbContext
    {
        public tdlDbContext() { }

        public tdlDbContext(DbContextOptions<tdlDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<TarefaOficial> TarefasOficial { get; set; }
        public DbSet<TarefaPersonalizada> TarefasPersonalizada { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioTarefasOficiais> UsuariosTarefasOficiais { get; set; }
        public DbSet<Compra> Compras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            // Produto
            modelBuilder.Entity<Produto>().HasKey(p => p.Id);
            modelBuilder.Entity<Produto>().Property(p => p.Id).IsRequired();

            // Tarefa Oficial
            modelBuilder.Entity<TarefaOficial>().HasKey(o => o.Id);
            modelBuilder.Entity<TarefaOficial>().Property(o => o.Id).IsRequired();

            // Tarefa Personalizada
            modelBuilder.Entity<TarefaPersonalizada>().HasKey(t => t.Id);
            modelBuilder.Entity<TarefaPersonalizada>().Property(t => t.Id).IsRequired();
            modelBuilder.Entity<TarefaPersonalizada>()
                .HasOne(tp => tp.Usuario)
                .WithMany(u => u.TarefasPersonalizadas)
                .HasForeignKey(tp => tp.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Usuario
            modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
            modelBuilder.Entity<Usuario>().Property(u => u.Id).IsRequired();

            // UsuarioTarefasOficiais
            modelBuilder.Entity<UsuarioTarefasOficiais>()
                .HasKey(x => new { x.UsuarioId, x.TarefaOficialId });

            modelBuilder.Entity<UsuarioTarefasOficiais>()
                .HasOne(x => x.Usuario)
                .WithMany(u => u.TarefasOficiais)
                .HasForeignKey(x => x.UsuarioId);

            modelBuilder.Entity<UsuarioTarefasOficiais>()
                .HasOne(x => x.TarefaOficial)
                .WithMany()
                .HasForeignKey(x => x.TarefaOficialId);

            // Compra
            modelBuilder.Entity<Compra>().HasKey(c => c.Id);

            modelBuilder.Entity<Compra>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Compra>()
                .HasOne<Produto>()
                .WithMany()
                .HasForeignKey(c => c.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Compra>().Property(c => c.Quantidade).IsRequired();
            modelBuilder.Entity<Compra>().Property(c => c.DataCompra).IsRequired();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
