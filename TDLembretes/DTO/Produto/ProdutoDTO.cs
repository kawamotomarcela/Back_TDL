namespace TDLembretes.DTO.Produto
{
    public class ProdutoDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int CustoEmPontos { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public string ImagemUrl { get; set; } = string.Empty;
    }
}
