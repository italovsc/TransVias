namespace TransVias.Models;

public class Veiculo
{
    public int Id { get; set; }

    // Relacionamento com Motorista
    public int MotoristaId { get; set; }
    public Motorista Motorista { get; set; } = null!;

    public string Placa { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string TipoVeiculo { get; set; } = string.Empty;

    public bool PossuiRastreador { get; set; }
    public string? TipoRastreador { get; set; }
    public string? EmpresaRastreador { get; set; }
}