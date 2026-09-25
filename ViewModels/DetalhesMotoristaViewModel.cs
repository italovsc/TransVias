using TransVias.Models;

namespace TransVias.ViewModels;

public class DetalhesMotoristaViewModel
{
    public Motorista Motorista { get; set; } = null!;

    public string Observacao { get; set; } = string.Empty;
}