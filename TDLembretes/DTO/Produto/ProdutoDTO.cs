namespace TDLembretes.DTO.Produto
{
    public class ProdutoDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int CustoEmPontos { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public string ImagemUrl { get; set; }

    }
}
