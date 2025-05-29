using TDLembretes.Models;
using Microsoft.EntityFrameworkCore;

namespace TDLembretes.Repositories.Data
{
    public class SeedData
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new tdlDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<tdlDbContext>>()))
            {
                if (!context.Usuarios.Any())
                {
                    context.Usuarios.AddRange(
                        new Usuario("1", "João da Silva", "joaosilva@gmail.com", "123", 5000, "1499123-5136", new List<UsuarioTarefasOficiais>()),
                        new Usuario("2", "Marcelao lindao", "marcelao@gmail.com", "001", 10000, "1496203-4215", new List<UsuarioTarefasOficiais>()),
                        new Usuario("3", "Junior Rodrigues", "junior@gmail.com", "121", 1500, "1497428-6342", new List<UsuarioTarefasOficiais>())
                    );
                    context.SaveChanges();
                }

                if (!context.Produtos.Any())
                {
                    context.Produtos.AddRange(
                        new Produto("301", "Cupom Mercado", "Cupom para usar no mercado Y", 1000, 3, "Exemplo Mercado"),
                        new Produto("303", "Cupom Roupa", "Cupom para comprar roupa na loja X", 2000, 3, "Exemplo Roupa")
                    );
                    context.SaveChanges();
                }

                if (!context.TarefasOficial.Any())
                {
                    context.TarefasOficial.AddRange(
                        new TarefaOficial("101", "Ler um livro", "Ler pelo menos um capítulo de um livro.", (PrioridadeTarefa)2, 1500, DateTime.UtcNow, DateTime.UtcNow.AddDays(3), (StatusTarefa)1, "Teste", (StatusComprovacao)1),
                        new TarefaOficial("102", "Praticar Exercício", "Praticar pelo menos 20min de atividade física.", (PrioridadeTarefa)1, 2000, DateTime.UtcNow, DateTime.UtcNow.AddDays(5), (StatusTarefa)1, "TESTE", (StatusComprovacao)1)
                    );
                    context.SaveChanges();
                }

                if (!context.TarefasPersonalizada.Any())
                {
                    context.TarefasPersonalizada.AddRange(
                        new TarefaPersonalizada("201", "Estudar programação", "Estudar C# por 1 hora", DateTime.UtcNow, DateTime.UtcNow.AddDays(3), (StatusTarefa)1, (PrioridadeTarefa)2, "1"),
                        new TarefaPersonalizada("202", "Arrumar o quarto", "Organizar livros e roupas", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), (StatusTarefa)1, (PrioridadeTarefa)1, "2"),
                        new TarefaPersonalizada("203", "Lavar a louça", "Lavar os pratos do almoço", DateTime.UtcNow, DateTime.UtcNow.AddDays(2), (StatusTarefa)1, (PrioridadeTarefa)1, "3")
                    );
                    context.SaveChanges();
                }

                if (!context.UsuariosTarefasOficiais.Any())
                {
                    context.UsuariosTarefasOficiais.AddRange(
                        new UsuarioTarefasOficiais("1", "101"),
                        new UsuarioTarefasOficiais("2", "102")
                     );
                    context.SaveChanges();

                

                }
            }
        }

    }
}
