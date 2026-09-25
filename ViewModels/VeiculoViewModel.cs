using System.ComponentModel.DataAnnotations;

namespace TransVias.ViewModels;

public class VeiculoViewModel
{
    [Required(ErrorMessage = "Informe a placa.")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o ano.")]
    public int? Ano { get; set; }

    [Required(ErrorMessage = "Informe a marca.")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o modelo.")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o tipo de veículo.")]
    public string TipoVeiculo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe se o veículo possui rastreador.")]
    public bool? PossuiRastreador { get; set; }

    public string? TipoRastreador { get; set; }

    public string? EmpresaRastreador { get; set; }
}