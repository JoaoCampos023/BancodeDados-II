using Microsoft.EntityFrameworkCore;
using SistemaAereo.Data;

namespace SistemaAereo.Services
{
    public static class DbInitializer
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AeroportoContext>();
                context.Database.EnsureCreated();

                // Opcional: Adicionar dados iniciais para teste
                SeedData(context);
            }
        }

        private static void SeedData(AeroportoContext context)
        {
            // Adicionar alguns dados de teste se o banco estiver vazio
            if (!context.Aeronaves.Any())
            {
                context.Aeronaves.AddRange(
                    new Models.Aeronave { TipoAeronave = "Boeing 737", NumeroPoltronas = 180 },
                    new Models.Aeronave { TipoAeronave = "Airbus A320", NumeroPoltronas = 150 }
                );
            }

            if (!context.Aeroportos.Any())
            {
                context.Aeroportos.AddRange(
                    new Models.Aeroporto { Nome = "Aeroporto Internacional do Rio de Janeiro", CodigoIATA = "GRU", Cidade = "Rio de Janeiro", Pais = "Brasil" },
                    new Models.Aeroporto { Nome = "Aeroporto Santos Dumont", CodigoIATA = "SDU", Cidade = "Rio de Janeiro", Pais = "Brasil" }
                );
            }

            context.SaveChanges();
        }
    }
}