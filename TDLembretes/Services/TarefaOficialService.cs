using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDLembretes.DTO.TarefaOficial;
using TDLembretes.Models;
using TDLembretes.Repositories;

namespace TDLembretes.Services
{
    public class TarefaOficialService
    {
        private readonly TarefaOficialRepository _tarefaOficialRepository;

        public TarefaOficialService(TarefaOficialRepository tarefaOficialRepository)
        {
            _tarefaOficialRepository = tarefaOficialRepository;
        }

        public async Task<string> CriarTarefaOficial(CriarTarefaOficialDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Titulo) ||
                string.IsNullOrWhiteSpace(dto.Descricao) ||
                dto.DataFinalizacao == default)
            {
                throw new ArgumentException("Todos os campos devem ser preenchidos corretamente.");
            }

            var novaTarefa = new TarefaOficial(
                Guid.NewGuid().ToString(),
                dto.Titulo,
                dto.Descricao,
                dto.Prioridade,
                dto.Pontos,
                DateTime.UtcNow,
                dto.DataFinalizacao,
                StatusTarefa.EmAndamento,
                null,
                StatusComprovacao.AguardandoAprovacao
            );

            await _tarefaOficialRepository.AddTarefaOficial(novaTarefa);
            return novaTarefa.Id;
        }

        public async Task UpdateTarefaOficial(string id, AtualizarTarefaOficialDTO dto)
        {
            var tarefa = await GetTarefaOficialOrThrowException(id);

            tarefa.Titulo = dto.Titulo;
            tarefa.Descricao = dto.Descricao;
            tarefa.Prioridade = dto.Prioridade;
            tarefa.DataFinalizacao = dto.DataFinalizacao;

            await _tarefaOficialRepository.UpdateTarefaOficial(tarefa);
        }

        public async Task EnviarComprovacao(string id, string comprovacaoUrl)
        {
            var tarefa = await GetTarefaOficialOrThrowException(id);
            tarefa.ComprovacaoUrl = comprovacaoUrl;
            await _tarefaOficialRepository.UpdateTarefaOficial(tarefa);
        }

        public async Task AtualizarStatus(string id, StatusTarefa status)
        {
            var tarefa = await GetTarefaOficialOrThrowException(id);
            tarefa.Status = status;
            await _tarefaOficialRepository.UpdateTarefaOficial(tarefa);
        }

        public async Task<TarefaOficial?> GetTarefaOficialPorId(string id)
        {
            return await _tarefaOficialRepository.GetTarefaOficial(id);
        }

        public async Task<TarefaOficial> GetTarefaOficialOrThrowException(string id)
        {
            var tarefa = await _tarefaOficialRepository.GetTarefaOficial(id);
            if (tarefa == null)
                throw new Exception("Tarefa oficial não encontrada.");

            return tarefa;
        }

        public async Task DeleteTarefaOficial(string id)
        {
            var tarefa = await GetTarefaOficialOrThrowException(id);
            await _tarefaOficialRepository.DeleteTarefaOficial(tarefa);
        }

        public async Task<List<TarefaOficialDTO>> GetTodasTarefasOficial()
        {
            var tarefas = await _tarefaOficialRepository.GetTodasTarefasOficial();

            return tarefas.Select(t => new TarefaOficialDTO
            {
                Id = t.Id, 
                Titulo = t.Titulo,
                Descricao = t.Descricao,
                Prioridade = t.Prioridade,
                Pontos = t.Pontos,
                DataCriacao = t.DataCriacao,
                DataFinalizacao = t.DataFinalizacao,
                ComprovacaoUrl = t.ComprovacaoUrl,
                Status = t.Status,
                StatusComprovacao = t.StatusComprovacao
            }).ToList();
        }

        public async Task<TarefaOficialDTO> GetTarefaOficialDTO(string id)
        {
            var tarefa = await GetTarefaOficialOrThrowException(id);

            return new TarefaOficialDTO
            {
                Id = tarefa.Id, 
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Prioridade = tarefa.Prioridade,
                Pontos = tarefa.Pontos,
                DataCriacao = tarefa.DataCriacao,
                DataFinalizacao = tarefa.DataFinalizacao,
                ComprovacaoUrl = tarefa.ComprovacaoUrl,
                Status = tarefa.Status,
                StatusComprovacao = tarefa.StatusComprovacao
            };
        }
    }
}
