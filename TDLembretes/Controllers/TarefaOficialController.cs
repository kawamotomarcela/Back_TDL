using Microsoft.AspNetCore.Mvc;
using TDLembretes.DTO.TarefaOficial;
using TDLembretes.Models;
using TDLembretes.Services;

namespace TDLembretes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarefaOficialController : ControllerBase
    {
        private readonly TarefaOficialService _tarefaOficialService;

        public TarefaOficialController(TarefaOficialService tarefaOficialService)
        {
            _tarefaOficialService = tarefaOficialService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarefaOficialDTO>>> GetTodos()
        {
            var tarefas = await _tarefaOficialService.GetTodasTarefasOficial();
            return Ok(tarefas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TarefaOficialDTO>> Get(string id)
        {
            try
            {
                var tarefa = await _tarefaOficialService.GetTarefaOficialDTO(id);
                return Ok(tarefa);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        [HttpPost("CriarTarefaOficial")]
        public async Task<ActionResult<string>> CriarTarefaOficial([FromBody] CriarTarefaOficialDTO dto)
        {
            try
            {
                var id = await _tarefaOficialService.CriarTarefaOficial(dto);
                return Ok(new { Message = "Tarefa criada com sucesso!", Id = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTarefaOficial(string id, [FromBody] AtualizarTarefaOficialDTO dto)
        {
            try
            {
                await _tarefaOficialService.UpdateTarefaOficial(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("comprovacao/{id}")]
        public async Task<IActionResult> EnviarComprovacao(string id, [FromBody] ComprovacaoURLDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ComprovacaoUrl))
                return BadRequest("A URL da imagem de comprovação é obrigatória.");

            try
            {
                await _tarefaOficialService.EnviarComprovacao(id, dto.ComprovacaoUrl);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("concluir/{id}")]
        public async Task<IActionResult> ConcluirTarefa(string id)
        {
            try
            {
                var tarefa = await _tarefaOficialService.GetTarefaOficialPorId(id);
                if (tarefa == null)
                    return NotFound("Tarefa não encontrada.");

                if (string.IsNullOrWhiteSpace(tarefa.ComprovacaoUrl))
                    return BadRequest("É necessário enviar uma imagem de comprovação antes de concluir.");

                await _tarefaOficialService.AtualizarStatus(id, StatusTarefa.Concluida);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarefaOficial(string id)
        {
            try
            {
                await _tarefaOficialService.DeleteTarefaOficial(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
