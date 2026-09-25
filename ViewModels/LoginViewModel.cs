using System.ComponentModel.DataAnnotations;

namespace TransVias.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    public string Senha { get; set; } = string.Empty;
}