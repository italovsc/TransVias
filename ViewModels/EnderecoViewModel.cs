using System.ComponentModel.DataAnnotations;

namespace TransVias.ViewModels;

public class EnderecoViewModel
{
    [Required(ErrorMessage = "Informe o CEP.")]
    public string Cep { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a rua ou avenida.")]
    public string RuaAvenida { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o número.")]
    public string Numero { get; set; } = string.Empty;

    public string? Complemento { get; set; }

    [Required(ErrorMessage = "Informe o bairro.")]
    public string Bairro { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a cidade.")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o estado.")]
    public string Estado { get; set; } = string.Empty;
}