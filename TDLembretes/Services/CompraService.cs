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
        /// Realiza uma compra de produto com pontos do usuário.
        /// </summary>
        public async Task<bool> RealizarCompra(string usuarioId, string produtoId, int quantidade)
        {
            if (quantidade <= 0)
                throw new Exception("A quantidade deve ser maior que zero.");

            var usuario = await _usuarioService.GetUsuarioOrThrowException(usuarioId);
            var produto = await _produtoService.GetProdutoOrThrowException(produtoId);

            if (produto.QuantidadeDisponivel < quantidade)
                throw new Exception("Estoque insuficiente para esta compra.");

            int custoTotal = produto.CustoEmPontos * quantidade;

            if (usuario.Pontos < custoTotal)
                throw new Exception("Pontos insuficientes para a compra.");

            // Atualiza saldo do usuário e estoque do produto
            usuario.Pontos -= custoTotal;
            produto.QuantidadeDisponivel -= quantidade;

            await _usuarioService.UpdateUsuarioOrThrowException(usuario);
            await _produtoService.UpdateProdutoOrThrowException(produto);

            // Registra a compra
            var novaCompra = new Compra
            {
                UsuarioId = usuarioId,
                ProdutoId = produtoId,
                Quantidade = quantidade,
                DataCompra = DateTime.UtcNow
            };

            await _compraRepository.AdicionarCompra(novaCompra);

            return true;
        }

        /// <summary>
        /// Retorna os produtos comprados por um usuário (como cupons).
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

