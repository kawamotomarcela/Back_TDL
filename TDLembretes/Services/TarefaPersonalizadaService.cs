using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TDLembretes.DTO.TarefaPersonalizada;
using TDLembretes.Models;
using TDLembretes.Repositories;

namespace TDLembretes.Services
{
    public class TarefaPersonalizadaService
    {

        private readonly TarefaPersonalizadaRepository _tarefaPersonalizadaRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TarefaPersonalizadaService(TarefaPersonalizadaRepository tarefaPersonalizadaRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tarefaPersonalizadaRepository = tarefaPersonalizadaRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        //POST
        public async Task<TarefaPersonalizada> CriarTarefaPersonalizada(CriarTarefaPersonalizadaDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Titulo) ||
                string.IsNullOrWhiteSpace(dto.Descricao) ||
                dto.DataFinalizacao == default)
            {
                throw new ArgumentException("Todos os campos devem ser preenchidos corretamente!");
            }

            var usuarioId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(usuarioId))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado ou sessão expirou.");
            }

            var novaTarefa = new TarefaPersonalizada(
                id: Guid.NewGuid().ToString(),
                titulo: dto.Titulo,
                descricao: dto.Descricao,
                dataCriacao: DateTime.UtcNow,
                dataFinalizacao: dto.DataFinalizacao,
                status: StatusTarefa.EmAndamento,
                prioridade: dto.Prioridade,
                usuarioId: usuarioId
            );

            await _tarefaPersonalizadaRepository.AddTarefaPersonalizada(novaTarefa);

            return novaTarefa;
        }



        //PUT
        public async Task UpdateTarefaPersonalizada(string id, AtualizarTarefaPersonalizadaDTO dto)
        {
            TarefaPersonalizada? tarefa = await _tarefaPersonalizadaRepository.GetTarefaPersonalizada(id);
            if (tarefa == null)
                throw new Exception("Tarefa não encontrada.");

            tarefa.Titulo = dto.Titulo;
            tarefa.Descricao = dto.Descricao;
            tarefa.Prioridade = dto.Prioridade;
            tarefa.DataFinalizacao = dto.DataFinalizacao;

            await _tarefaPersonalizadaRepository.UpdateTarefaPersonalizada(tarefa);
        }

        public async Task UpdateStatusTarefaPersonalizada(string id, AtualizarStatusPersonalizadaDTO statusDto)
        {
            TarefaPersonalizada? tarefa = await _tarefaPersonalizadaRepository.GetTarefaPersonalizada(id);
            if (tarefa == null)
                throw new Exception("Tarefa não encontrada.");

            // Loga o horário atual do servidor (UTC)
            Console.WriteLine("🕒 Agora (UTC): " + DateTime.UtcNow);
            Console.WriteLine("📅 DataFinalizacao da tarefa: " + tarefa.DataFinalizacao);

            if (DateTime.UtcNow > tarefa.DataFinalizacao)
            {
                tarefa.Status = StatusTarefa.Expirada;
            }
            else
            {
                tarefa.Status = statusDto.Status == StatusTarefa.Concluida
                                ? StatusTarefa.Concluida
                                : StatusTarefa.EmAndamento;
            }

            await _tarefaPersonalizadaRepository.UpdateTarefaPersonalizada(tarefa);
        }


        //DELET
        public async Task DeleteTarefaPersonalizada(string id)
        {
            TarefaPersonalizada? tarefa = await _tarefaPersonalizadaRepository.GetTarefaPersonalizada(id);
            if (tarefa == null)
                throw new Exception("Tarefa não encontrada.");

            await _tarefaPersonalizadaRepository.DeleteTarefaPersonalizada(tarefa);
        }
        public async Task<IEnumerable<TarefaPersonalizada>> GetTarefasPorUsuarioAsync()
        {
            var usuarioId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(usuarioId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var tarefas = await _tarefaPersonalizadaRepository.GetTarefasPorUsuarioAsync(usuarioId);
            var agoraUtc = DateTime.UtcNow;

            foreach (var tarefa in tarefas)
            {
                if (tarefa.Status == StatusTarefa.EmAndamento && agoraUtc > tarefa.DataFinalizacao)
                {
                    tarefa.Status = StatusTarefa.Expirada;
                    await _tarefaPersonalizadaRepository.UpdateTarefaPersonalizada(tarefa);

                    Console.WriteLine($"⚠️ Tarefa '{tarefa.Titulo}' marcada como expirada (vencia em {tarefa.DataFinalizacao})");
                }
            }

            return tarefas;
        }

    }
}
