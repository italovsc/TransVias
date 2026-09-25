using System.ComponentModel.DataAnnotations;

namespace TransVias.ViewModels;

public class CriarContaViewModel
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a senha.")]
    [Compare(
        nameof(Senha),
        ErrorMessage = "As senhas não coincidem."
    )]
    public string ConfirmarSenha { get; set; } = string.Empty;
}