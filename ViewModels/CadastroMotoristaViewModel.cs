using System.ComponentModel.DataAnnotations;

namespace TransVias.ViewModels;

public class CadastroMotoristaViewModel
{
    [Required(ErrorMessage = "Informe o nome completo.")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o CPF.")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a data de nascimento.")]
    public DateTime? DataNascimento { get; set; }

    [Required(ErrorMessage = "Informe o telefone principal.")]
    public string TelefonePrincipal { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o telefone secundário.")]
    public string TelefoneSecundario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o número da CNH.")]
    public string NumeroCnh { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione a categoria da CNH.")]
    public string CategoriaCnh { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe se possui certificação especial.")]
    public bool? PossuiCertificacaoEspecial { get; set; }

    public string? TipoCertificacao { get; set; }
}