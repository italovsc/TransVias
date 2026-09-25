using TransVias.Models.Enums;

namespace TransVias.ViewModels;

public class MotoristaResumoViewModel
{
    public int Id { get; set; }

    public string NomeCompleto { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;

    public DateTime DataEnvio { get; set; }

    public StatusCadastro Status { get; set; }
}