using TDLembretes.Models;
using TDLembretes.Repositories.Data;
using TDLembretes.DTO.Produto;

namespace TDLembretes.Services
{
    public class CompraService
    {
        private readonly UsuarioService _usuarioService;
        private readonly ProdutoService _produtoService;
        private readonly CompraRepository _compraRepository;

        public CompraService(
            UsuarioService usuarioService,
            ProdutoService produtoService,
            CompraRepository compraRepository)
        {
            _usuarioService = usuarioService;
            _produtoService = produtoService;
            _compraRepository = compraRepository;
        }

        /// <summary>
        /// Realiza uma compra: atualiza pontos, estoque e salva o registro da compra.
        /// </summary>
        public async Task<bool> RealizarCompra(string usuarioId, string produtoId, int quantidade)
        {
            var usuario = await _usuarioService.GetUsuarioOrThrowException(usuarioId);
            var produto = await _produtoService.GetProdutoOrThrowException(produtoId);

            if (usuario == null || produto == null)
                throw new Exception("Usuário ou produto não encontrado.");

            int custoTotal = produto.CustoEmPontos * quantidade;

            if (usuario.Pontos < custoTotal)
                throw new Exception("Pontos insuficientes para a compra.");

            // Atualiza saldo e estoque
            usuario.Pontos -= custoTotal;
            produto.QuantidadeDisponivel -= quantidade;

            await _usuarioService.UpdateUsuarioOrThrowException(usuario);
            await _produtoService.UpdateProdutoOrThrowException(produto);

            // Registra a compra
            var novaCompra = new Compra
            {
                UsuarioId = usuarioId,
                ProdutoId = produtoId,
                Quantidade = quantidade
            };

            await _compraRepository.AdicionarCompra(novaCompra);

            return true;
        }

        /// <summary>
        /// Retorna os produtos comprados por um usuário (para exibir como cupons).
        /// </summary>
        public async Task<List<ProdutoDTO>> GetProdutosComprados(string usuarioId)
        {
            var compras = await _compraRepository.GetComprasPorUsuario(usuarioId);
            var produtos = new List<ProdutoDTO>();

            foreach (var compra in compras)
            {
                var produto = await _produtoService.GetProdutoOrThrowException(compra.ProdutoId);
                if (produto != null)
                {
                    produtos.Add(new ProdutoDTO
                    {
                        Id = produto.Id,
                        Nome = produto.Nome,
                        Descricao = produto.Descricao,
                        CustoEmPontos = produto.CustoEmPontos,
                        QuantidadeDisponivel = produto.QuantidadeDisponivel,
                        ImagemUrl = produto.ImagemUrl
                    });
                }
            }

            return produtos;
        }
    }
}

