namespace TransVias.ViewModels;

public class RevisaoCadastroViewModel
{
    public CadastroMotoristaViewModel DadosPessoais { get; set; } = new();

    public EnderecoViewModel Endereco { get; set; } = new();

    public VeiculoViewModel Veiculo { get; set; } = new();
}