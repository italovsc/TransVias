using Microsoft.EntityFrameworkCore;
using TransVias.Data;
using TransVias.Models;
using TransVias.Models.Enums;
using TransVias.Repositories.Interfaces;

namespace TransVias.Repositories;

public class MotoristaRepository : IMotoristaRepository
{
    private readonly AppDbContext _context;

    public MotoristaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Motorista?> BuscarPorIdAsync(int id)
    {
        return await _context.Motoristas
            .Include(m => m.Usuario)
            .Include(m => m.Endereco)
            .Include(m => m.Veiculo)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Motorista?> BuscarPorUsuarioIdAsync(int usuarioId)
    {
        return await _context.Motoristas
            .Include(m => m.Usuario)
            .Include(m => m.Endereco)
            .Include(m => m.Veiculo)
            .FirstOrDefaultAsync(m => m.UsuarioId == usuarioId);
    }

    public async Task<List<Motorista>> ListarTodosAsync()
    {
        return await _context.Motoristas
            .Include(m => m.Usuario)
            .Include(m => m.Endereco)
            .Include(m => m.Veiculo)
            .OrderByDescending(m => m.DataEnvio)
            .ToListAsync();
    }

    public async Task<int> ContarPorStatusAsync(StatusCadastro status)
    {
        return await _context.Motoristas
            .CountAsync(m => m.Status == status);
    }

    public async Task AdicionarAsync(Motorista motorista)
    {
        await _context.Motoristas.AddAsync(motorista);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Motorista motorista)
    {
        _context.Motoristas.Update(motorista);
        await _context.SaveChangesAsync();
    }
}