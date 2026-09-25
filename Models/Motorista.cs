using TransVias.Models.Enums;

namespace TransVias.Models;

public class Motorista
{
    public int Id { get; set; }

    // Relacionamento com Usuario
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    // Dados pessoais
    public string NomeCompleto { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }

    // Contato
    public string TelefonePrincipal { get; set; } = string.Empty;
    public string TelefoneSecundario { get; set; } = string.Empty;

    // CNH
    public string NumeroCnh { get; set; } = string.Empty;
    public string CategoriaCnh { get; set; } = string.Empty;

    // Certificação
    public bool PossuiCertificacaoEspecial { get; set; }
    public string? TipoCertificacao { get; set; }

    // Processo de análise
    public StatusCadastro Status { get; set; }
    public string? ObservacaoAnalise { get; set; }

    public DateTime DataEnvio { get; set; }
    public DateTime DataAtualizacao { get; set; }

    // Relacionamentos
    public Endereco Endereco { get; set; } = null!;
    public Veiculo Veiculo { get; set; } = null!;
}