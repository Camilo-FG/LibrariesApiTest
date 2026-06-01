using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryService.WebAPI.Data
{
    public static class LibraryDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();

            if (!await context.Libraries.AnyAsync())
            {
                await context.Libraries.AddRangeAsync(
                    new Library { Name = "Biblioteca Central", Location = "San José" },
                    new Library { Name = "Biblioteca Norte", Location = "Heredia" },
                    new Library { Name = "Biblioteca Pacífico", Location = "Puntarenas" },
                    new Library { Name = "Biblioteca Caribe", Location = "Limón" }
                );
                await context.SaveChangesAsync();
            }

            if (await context.Books.AnyAsync())
                return;

            var central = await context.Libraries.FirstOrDefaultAsync(l => l.Name == "Biblioteca Central");
            var norte = await context.Libraries.FirstOrDefaultAsync(l => l.Name == "Biblioteca Norte");

            if (central != null)
            {
                await context.Books.AddRangeAsync(
                    new Book { Name = "Cien años de soledad", Category = "Novela", LibraryId = central.Id },
                    new Book { Name = "El señor de los anillos", Category = "Fantasía", LibraryId = central.Id }
                );
            }

            if (norte != null)
            {
                await context.Books.AddRangeAsync(
                    new Book { Name = "Clean Code", Category = "Tecnología", LibraryId = norte.Id },
                    new Book { Name = "Introducción a algoritmos", Category = "Tecnología", LibraryId = norte.Id }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
