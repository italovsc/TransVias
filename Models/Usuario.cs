using TransVias.Models.Enums;

namespace TransVias.Models;

public class Usuario
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public TipoUsuario TipoUsuario { get; set; }

    public Motorista? Motorista { get; set; }
}