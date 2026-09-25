using Microsoft.AspNetCore.Identity;
using TransVias.Models;
using TransVias.Models.Enums;
using TransVias.Repositories.Interfaces;

namespace TransVias.Services;

public class AccountService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public AccountService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    public async Task<(Usuario? Usuario, string? Erro)> CriarContaAsync(
        string email,
        string senha,
        string confirmarSenha)
    {
        email = email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            return (null, "E-mail e senha são obrigatórios.");
        }

        if (senha != confirmarSenha)
        {
            return (null, "As senhas não coincidem.");
        }

        if (await _usuarioRepository.EmailExisteAsync(email))
        {
            return (null, "Já existe uma conta cadastrada com este e-mail.");
        }

        var usuario = new Usuario
        {
            Email = email,
            TipoUsuario = TipoUsuario.Motorista
        };

        usuario.SenhaHash = _passwordHasher.HashPassword(usuario, senha);

        await _usuarioRepository.AdicionarAsync(usuario);

        return (usuario, null);
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        email = email.Trim().ToLowerInvariant();

        var usuario = await _usuarioRepository.BuscarPorEmailAsync(email);

        if (usuario is null)
        {
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            senha
        );

        if (resultado == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return usuario;
    }
}