using Microsoft.AspNetCore.Mvc;
using TDLembretes.Services;
using TDLembretes.DTO.Compra;
using TDLembretes.DTO.Produto;

namespace TDLembretes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly CompraService _compraService;

        public CompraController(CompraService compraService)
        {
            _compraService = compraService;
        }

        /// <summary>
        /// Realiza uma compra de produto com pontos do usuário.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RealizarCompra([FromBody] ComprarRequest request)
        {
            try
            {
                var sucesso = await _compraService.RealizarCompra(
                    request.UsuarioId,
                    request.ProdutoId,
                    request.Quantidade
                );

                return Ok(new
                {
                    mensagem = "Compra realizada com sucesso.",
                    sucesso
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Retorna os produtos já comprados pelo usuário (ex: cupons).
        /// </summary>
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetCuponsDoUsuario(string usuarioId)
        {
            try
            {
                List<ProdutoDTO> produtos = await _compraService.GetProdutosComprados(usuarioId);

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
