using TransVias.Models;
using TransVias.Models.Enums;

namespace TransVias.Repositories.Interfaces;

public interface IMotoristaRepository
{
    Task<Motorista?> BuscarPorIdAsync(int id);

    Task<Motorista?> BuscarPorUsuarioIdAsync(int usuarioId);

    Task<List<Motorista>> ListarTodosAsync();

    Task<int> ContarPorStatusAsync(StatusCadastro status);

    Task AdicionarAsync(Motorista motorista);

    Task AtualizarAsync(Motorista motorista);
}