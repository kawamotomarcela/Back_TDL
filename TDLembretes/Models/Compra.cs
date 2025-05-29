namespace TDLembretes.Models
{
    public class Compra
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UsuarioId { get; set; } = string.Empty;
        public string ProdutoId { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public DateTime DataCompra { get; set; } = DateTime.UtcNow;
    }

}
