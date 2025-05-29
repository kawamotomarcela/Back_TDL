using Microsoft.EntityFrameworkCore;
using TDLembretes.Models;
using TDLembretes.Repositories.Data;
using System;

namespace TDLembretes.Repositories.Data
{
    public class CompraRepository
    {
        private readonly tdlDbContext _context;

        public CompraRepository(tdlDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarCompra(Compra compra)
        {
            await _context.Compras.AddAsync(compra);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Compra>> GetComprasPorUsuario(string usuarioId)
        {
            return await _context.Compras
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();
        }
    }
}
