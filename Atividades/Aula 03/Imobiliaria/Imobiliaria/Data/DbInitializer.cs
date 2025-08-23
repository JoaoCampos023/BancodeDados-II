using Imobiliaria.Models;
using Imobiliaria.Data;

namespace Imobiliaria.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Imoveis.Any())
            {
                return;
            }

            var imoveis = new Imovel[]
            {
                new Imovel{Name="Apartamento",Description="Apartamento Grande",NumberRoom=8,SalePrice=1000000},
                new Imovel{Name="Casa",Description="Casa Grande",NumberRoom=8,SalePrice=20000000},
                new Imovel{Name="Casa",Description="Casa Media",NumberRoom=4,SalePrice=200000}
            };
            foreach (Imovel s in imoveis)
            {
                context.Imoveis.Add(s);
            }
            context.SaveChanges();
        }
    }
}
