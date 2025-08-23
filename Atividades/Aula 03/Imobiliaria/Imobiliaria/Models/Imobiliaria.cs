namespace Imobiliaria.Models
{
    public class Imovel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; } 
        public int NumberRoom { get; set; }
        public decimal SalePrice { get; set; }

    }
}
