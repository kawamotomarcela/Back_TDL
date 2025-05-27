using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TDLembretes.DTO.TarefaPersonalizada;
using TDLembretes.Models;
using TDLembretes.Repositories.Data;
using TDLembretes.Services;

namespace TDLembretes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarefaPersonalizadaController : ControllerBase

    {
        private readonly TarefaPersonalizadaService _tarefaPersonalizadaService;

        public TarefaPersonalizadaController(TarefaPersonalizadaService tarefaPersonalizadaService)
        {
            _tarefaPersonalizadaService = tarefaPersonalizadaService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarefaPersonalizada>>> GetTarefasDoUsuario()
        {
            try
            {
                var tarefas = await _tarefaPersonalizadaService.GetTarefasPorUsuarioAsync();
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult<TarefaPersonalizada>> CriarTarefaPersonalizada([FromBody] CriarTarefaPersonalizadaDTO dto)
        {
            try
            {
                var tarefa = await _tarefaPersonalizadaService.CriarTarefaPersonalizada(dto);
                return CreatedAtAction(nameof(GetTarefasDoUsuario), new { id = tarefa.Id }, tarefa);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTarefaPersonalizada(string id, [FromBody] AtualizarTarefaPersonalizadaDTO dto)
        {
            await _tarefaPersonalizadaService.UpdateTarefaPersonalizada(id, dto);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] AtualizarStatusPersonalizadaDTO dto)
        {
            try
            {
                await _tarefaPersonalizadaService.UpdateStatusTarefaPersonalizada(id, dto);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarefaPersonalizada(string id)
        {
            try
            {
                await _tarefaPersonalizadaService.DeleteTarefaPersonalizada(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


    }
}