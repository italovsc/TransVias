using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TransVias.Models;
using TransVias.Models.Enums;

namespace TransVias.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Garante que as migrations existentes sejam aplicadas.
        await context.Database.MigrateAsync();


        // Verifica se já existe algum administrador.
        var adminExiste = await context.Usuarios
            .AnyAsync(u =>
                u.TipoUsuario == TipoUsuario.Administrador);

        if (adminExiste)
        {
            return;
        }


        // Busca as credenciais da configuração.
        var email = configuration["AdminInicial:Email"];
        var senha = configuration["AdminInicial:Senha"];


        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException(
                "As credenciais do administrador inicial não foram configuradas."
            );
        }


        email = email.Trim().ToLowerInvariant();


        // Evita conflito caso exista um motorista com o mesmo e-mail.
        var emailJaUtilizado = await context.Usuarios
            .AnyAsync(u => u.Email == email);

        if (emailJaUtilizado)
        {
            throw new InvalidOperationException(
                $"O e-mail '{email}' já está sendo utilizado."
            );
        }


        // Cria objeto
        var admin = new Usuario
        {
            Email = email,
            TipoUsuario = TipoUsuario.Administrador
        };


        // Gerar hash
        var passwordHasher = new PasswordHasher<Usuario>();

        admin.SenhaHash = passwordHasher.HashPassword(
            admin,
            senha
        );


        // Salva
        await context.Usuarios.AddAsync(admin);

        await context.SaveChangesAsync();
    }
}