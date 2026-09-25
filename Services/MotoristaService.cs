using TransVias.Models;
using TransVias.Models.Enums;
using TransVias.Repositories.Interfaces;

namespace TransVias.Services;

public class MotoristaService
{
    private readonly IMotoristaRepository _motoristaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public MotoristaService(
        IMotoristaRepository motoristaRepository,
        IUsuarioRepository usuarioRepository)
    {
        _motoristaRepository = motoristaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<(Motorista? Motorista, string? Erro)> CriarCadastroAsync(
        int usuarioId,
        Motorista motorista)
    {
        var usuario = await _usuarioRepository.BuscarPorIdAsync(usuarioId);

        if (usuario is null)
        {
            return (null, "Usuário não encontrado.");
        }

        if (usuario.TipoUsuario != TipoUsuario.Motorista)
        {
            return (null, "Apenas usuários motoristas podem realizar um cadastro.");
        }

        var cadastroExistente =
            await _motoristaRepository.BuscarPorUsuarioIdAsync(usuarioId);

        if (cadastroExistente is not null)
        {
            return (null, "Este usuário já possui um cadastro de motorista.");
        }

        var erroCondicional = ValidarDadosCondicionais(motorista);

        if (erroCondicional is not null)
        {
            return (null, erroCondicional);
        }

        motorista.UsuarioId = usuarioId;
        motorista.Status = StatusCadastro.Pendente;
        motorista.ObservacaoAnalise = null;
        motorista.DataEnvio = DateTime.Now;
        motorista.DataAtualizacao = DateTime.Now;

        NormalizarDadosCondicionais(motorista);

        await _motoristaRepository.AdicionarAsync(motorista);

        return (motorista, null);
    }

    public async Task<Motorista?> BuscarPorUsuarioIdAsync(int usuarioId)
    {
        return await _motoristaRepository.BuscarPorUsuarioIdAsync(usuarioId);
    }

    public async Task<Motorista?> BuscarPorIdAsync(int motoristaId)
    {
        return await _motoristaRepository.BuscarPorIdAsync(motoristaId);
    }

    public async Task<List<Motorista>> ListarTodosAsync()
    {
        return await _motoristaRepository.ListarTodosAsync();
    }

    public async Task<(int Pendentes, int Aprovados, int Recusados)>
        ObterResumoDashboardAsync()
    {
        var pendentes = await _motoristaRepository
            .ContarPorStatusAsync(StatusCadastro.Pendente);

        var aprovados = await _motoristaRepository
            .ContarPorStatusAsync(StatusCadastro.Aprovado);

        var recusados = await _motoristaRepository
            .ContarPorStatusAsync(StatusCadastro.Recusado);

        return (pendentes, aprovados, recusados);
    }

    public async Task<(bool Sucesso, string? Erro)> ReenviarCorrecaoAsync(
    int usuarioId,
    Motorista dadosAtualizados)
{
    var motorista =
        await _motoristaRepository.BuscarPorUsuarioIdAsync(usuarioId);

    if (motorista is null)
    {
        return (
            false,
            "Cadastro do motorista não encontrado."
        );
    }

    if (motorista.Status != StatusCadastro.CorrecaoSolicitada)
    {
        return (
            false,
            "Este cadastro não está disponível para correção."
        );
    }

    var erroCondicional =
        ValidarDadosCondicionais(dadosAtualizados);

    if (erroCondicional is not null)
    {
        return (
            false,
            erroCondicional
        );
    }


    // =========================
    // DADOS PESSOAIS
    // =========================

    motorista.NomeCompleto =
        dadosAtualizados.NomeCompleto;

    motorista.Cpf =
        dadosAtualizados.Cpf;

    motorista.DataNascimento =
        dadosAtualizados.DataNascimento;

    motorista.TelefonePrincipal =
        dadosAtualizados.TelefonePrincipal;

    motorista.TelefoneSecundario =
        dadosAtualizados.TelefoneSecundario;

    motorista.NumeroCnh =
        dadosAtualizados.NumeroCnh;

    motorista.CategoriaCnh =
        dadosAtualizados.CategoriaCnh;

    motorista.PossuiCertificacaoEspecial =
        dadosAtualizados.PossuiCertificacaoEspecial;

    motorista.TipoCertificacao =
        dadosAtualizados.TipoCertificacao;


    // =========================
    // ENDEREÇO
    // =========================

    motorista.Endereco.Cep =
        dadosAtualizados.Endereco.Cep;

    motorista.Endereco.RuaAvenida =
        dadosAtualizados.Endereco.RuaAvenida;

    motorista.Endereco.Numero =
        dadosAtualizados.Endereco.Numero;

    motorista.Endereco.Complemento =
        dadosAtualizados.Endereco.Complemento;

    motorista.Endereco.Bairro =
        dadosAtualizados.Endereco.Bairro;

    motorista.Endereco.Cidade =
        dadosAtualizados.Endereco.Cidade;

    motorista.Endereco.Estado =
        dadosAtualizados.Endereco.Estado;


    // =========================
    // VEÍCULO
    // =========================

    motorista.Veiculo.Placa =
        dadosAtualizados.Veiculo.Placa;

    motorista.Veiculo.Ano =
        dadosAtualizados.Veiculo.Ano;

    motorista.Veiculo.Marca =
        dadosAtualizados.Veiculo.Marca;

    motorista.Veiculo.Modelo =
        dadosAtualizados.Veiculo.Modelo;

    motorista.Veiculo.TipoVeiculo =
        dadosAtualizados.Veiculo.TipoVeiculo;

    motorista.Veiculo.PossuiRastreador =
        dadosAtualizados.Veiculo.PossuiRastreador;

    motorista.Veiculo.TipoRastreador =
        dadosAtualizados.Veiculo.TipoRastreador;

    motorista.Veiculo.EmpresaRastreador =
        dadosAtualizados.Veiculo.EmpresaRastreador;


    // =========================
    // VOLTA PARA ANÁLISE
    // =========================

    motorista.Status = StatusCadastro.Pendente;

    motorista.ObservacaoAnalise = null;

    motorista.DataAtualizacao = DateTime.Now;


    NormalizarDadosCondicionais(motorista);

    await _motoristaRepository.AtualizarAsync(motorista);

    return (true, null);
}

    public async Task<(bool Sucesso, string? Erro)> AprovarAsync(int motoristaId)
    {
        var motorista = await _motoristaRepository.BuscarPorIdAsync(motoristaId);

        if (motorista is null)
        {
            return (false, "Motorista não encontrado.");
        }

        if (motorista.Status != StatusCadastro.Pendente)
        {
            return (false, "Apenas cadastros pendentes podem ser aprovados.");
        }

        motorista.Status = StatusCadastro.Aprovado;
        motorista.ObservacaoAnalise = null;
        motorista.DataAtualizacao = DateTime.Now;

        await _motoristaRepository.AtualizarAsync(motorista);

        return (true, null);
    }

    public async Task<(bool Sucesso, string? Erro)> RecusarAsync(
        int motoristaId,
        string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
        {
            return (false, "Informe o motivo da recusa.");
        }

        var motorista = await _motoristaRepository.BuscarPorIdAsync(motoristaId);

        if (motorista is null)
        {
            return (false, "Motorista não encontrado.");
        }

        if (motorista.Status != StatusCadastro.Pendente)
        {
            return (false, "Apenas cadastros pendentes podem ser recusados.");
        }

        motorista.Status = StatusCadastro.Recusado;
        motorista.ObservacaoAnalise = motivo.Trim();
        motorista.DataAtualizacao = DateTime.Now;

        await _motoristaRepository.AtualizarAsync(motorista);

        return (true, null);
    }

    public async Task<(bool Sucesso, string? Erro)> SolicitarCorrecaoAsync(
        int motoristaId,
        string observacao)
    {
        if (string.IsNullOrWhiteSpace(observacao))
        {
            return (false, "Informe o que precisa ser corrigido.");
        }

        var motorista = await _motoristaRepository.BuscarPorIdAsync(motoristaId);

        if (motorista is null)
        {
            return (false, "Motorista não encontrado.");
        }

        if (motorista.Status != StatusCadastro.Pendente)
        {
            return (
                false,
                "Apenas cadastros pendentes podem receber solicitação de correção."
            );
        }

        motorista.Status = StatusCadastro.CorrecaoSolicitada;
        motorista.ObservacaoAnalise = observacao.Trim();
        motorista.DataAtualizacao = DateTime.Now;

        await _motoristaRepository.AtualizarAsync(motorista);

        return (true, null);
    }

    private static string? ValidarDadosCondicionais(Motorista motorista)
    {
        if (motorista.PossuiCertificacaoEspecial &&
            string.IsNullOrWhiteSpace(motorista.TipoCertificacao))
        {
            return "Informe o tipo da certificação especial.";
        }

        if (motorista.Veiculo.PossuiRastreador &&
            (string.IsNullOrWhiteSpace(motorista.Veiculo.TipoRastreador) ||
             string.IsNullOrWhiteSpace(motorista.Veiculo.EmpresaRastreador)))
        {
            return "Informe os dados do rastreador.";
        }

        return null;
    }

    private static void NormalizarDadosCondicionais(Motorista motorista)
    {
        if (!motorista.PossuiCertificacaoEspecial)
        {
            motorista.TipoCertificacao = null;
        }

        if (!motorista.Veiculo.PossuiRastreador)
        {
            motorista.Veiculo.TipoRastreador = null;
            motorista.Veiculo.EmpresaRastreador = null;
        }
    }
}