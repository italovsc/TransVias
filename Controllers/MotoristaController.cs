using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransVias.Models;
using TransVias.Models.Enums;
using TransVias.Services;
using TransVias.ViewModels;

namespace TransVias.Controllers;

[Authorize(Roles = "Motorista")]
public class MotoristaController : Controller
{
    private readonly MotoristaService _motoristaService;

    private const string DadosPessoaisSessionKey = "Cadastro_DadosPessoais";
    private const string EnderecoSessionKey = "Cadastro_Endereco";
    private const string VeiculoSessionKey = "Cadastro_Veiculo";

    public MotoristaController(MotoristaService motoristaService)
    {
        _motoristaService = motoristaService;
    }

    // =====================================================
    // ETAPA 1 - DADOS PESSOAIS
    // =====================================================

    [HttpGet]
    public IActionResult DadosPessoais()
    {
        var model = ObterDaSession<CadastroMotoristaViewModel>(
            DadosPessoaisSessionKey
        );

        return View(model ?? new CadastroMotoristaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DadosPessoais(CadastroMotoristaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        SalvarNaSession(DadosPessoaisSessionKey, model);

        return RedirectToAction(nameof(Endereco));
    }

    // =====================================================
    // ETAPA 2 - ENDEREÇO
    // =====================================================

    [HttpGet]
    public IActionResult Endereco()
    {
        if (!ExisteNaSession(DadosPessoaisSessionKey))
        {
            return RedirectToAction(nameof(DadosPessoais));
        }

        var model = ObterDaSession<EnderecoViewModel>(
            EnderecoSessionKey
        );

        return View(model ?? new EnderecoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Endereco(EnderecoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        SalvarNaSession(EnderecoSessionKey, model);

        return RedirectToAction(nameof(Veiculo));
    }

    // =====================================================
    // ETAPA 3 - VEÍCULO
    // =====================================================

    [HttpGet]
    public IActionResult Veiculo()
    {
        if (!ExisteNaSession(DadosPessoaisSessionKey) ||
            !ExisteNaSession(EnderecoSessionKey))
        {
            return RedirectToAction(nameof(DadosPessoais));
        }

        var model = ObterDaSession<VeiculoViewModel>(
            VeiculoSessionKey
        );

        return View(model ?? new VeiculoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Veiculo(VeiculoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        SalvarNaSession(VeiculoSessionKey, model);

        return RedirectToAction(nameof(Revisao));
    }

    // =====================================================
    // ETAPA 4 - REVISÃO
    // =====================================================

    [HttpGet]
    public IActionResult Revisao()
    {
        var dadosPessoais =
            ObterDaSession<CadastroMotoristaViewModel>(
                DadosPessoaisSessionKey
            );

        var endereco =
            ObterDaSession<EnderecoViewModel>(
                EnderecoSessionKey
            );

        var veiculo =
            ObterDaSession<VeiculoViewModel>(
                VeiculoSessionKey
            );

        if (dadosPessoais is null ||
            endereco is null ||
            veiculo is null)
        {
            return RedirectToAction(nameof(DadosPessoais));
        }

        var model = new RevisaoCadastroViewModel
        {
            DadosPessoais = dadosPessoais,
            Endereco = endereco,
            Veiculo = veiculo
        };

        return View(model);
    }

    // =====================================================
    // ENVIO DO CADASTRO
    // =====================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnviarCadastro()
    {
        var dadosPessoais =
            ObterDaSession<CadastroMotoristaViewModel>(
                DadosPessoaisSessionKey
            );

        var enderecoModel =
            ObterDaSession<EnderecoViewModel>(
                EnderecoSessionKey
            );

        var veiculoModel =
            ObterDaSession<VeiculoViewModel>(
                VeiculoSessionKey
            );

        if (dadosPessoais is null ||
            enderecoModel is null ||
            veiculoModel is null)
        {
            return RedirectToAction(nameof(DadosPessoais));
        }

        var usuarioId = ObterUsuarioId();

        if (usuarioId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var motorista = new Motorista
        {
            NomeCompleto = dadosPessoais.NomeCompleto,
            Cpf = dadosPessoais.Cpf,
            DataNascimento = dadosPessoais.DataNascimento!.Value,

            TelefonePrincipal = dadosPessoais.TelefonePrincipal,
            TelefoneSecundario = dadosPessoais.TelefoneSecundario,

            NumeroCnh = dadosPessoais.NumeroCnh,
            CategoriaCnh = dadosPessoais.CategoriaCnh,

            PossuiCertificacaoEspecial =
                dadosPessoais.PossuiCertificacaoEspecial!.Value,

            TipoCertificacao =
                dadosPessoais.TipoCertificacao,

            Endereco = new Endereco
            {
                Cep = enderecoModel.Cep,
                RuaAvenida = enderecoModel.RuaAvenida,
                Numero = enderecoModel.Numero,
                Complemento = enderecoModel.Complemento,
                Bairro = enderecoModel.Bairro,
                Cidade = enderecoModel.Cidade,
                Estado = enderecoModel.Estado
            },

            Veiculo = new Veiculo
            {
                Placa = veiculoModel.Placa,
                Ano = veiculoModel.Ano!.Value,
                Marca = veiculoModel.Marca,
                Modelo = veiculoModel.Modelo,
                TipoVeiculo = veiculoModel.TipoVeiculo,

                PossuiRastreador =
                    veiculoModel.PossuiRastreador!.Value,

                TipoRastreador =
                    veiculoModel.TipoRastreador,

                EmpresaRastreador =
                    veiculoModel.EmpresaRastreador
            }
        };

                var cadastroExistente =
    await _motoristaService.BuscarPorUsuarioIdAsync(
        usuarioId.Value
    );


string? erro;


        if (cadastroExistente is null)
        {
            // Primeiro cadastro do motorista
            var resultadoCriacao =
                await _motoristaService.CriarCadastroAsync(
                    usuarioId.Value,
                    motorista
                );

            erro = resultadoCriacao.Erro;
        }
        else if (
            cadastroExistente.Status ==
            StatusCadastro.CorrecaoSolicitada)
        {
            // Cadastro já existe e está liberado para correção
            var resultadoCorrecao =
                await _motoristaService.ReenviarCorrecaoAsync(
                    usuarioId.Value,
                    motorista
                );

            erro = resultadoCorrecao.Erro;
        }
        else
        {
            // Pendente, aprovado ou recusado não podem ser editados
            erro =
                "Este cadastro não pode ser alterado no momento.";
        }


        if (erro is not null)
        {
            TempData["Erro"] = erro;

            return RedirectToAction(nameof(Revisao));
        }


        LimparCadastroDaSession();

return RedirectToAction(nameof(Painel));
    }

    // =====================================================
    // PAINEL DO MOTORISTA
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> Painel()
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var motorista =
            await _motoristaService.BuscarPorUsuarioIdAsync(
                usuarioId.Value
            );

        if (motorista is null)
        {
            return RedirectToAction(nameof(DadosPessoais));
        }

        var model = new PainelMotoristaViewModel
        {
            Motorista = motorista
        };

        return View(model);
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> IniciarCorrecao()
{
    var usuarioId = ObterUsuarioId();

    if (usuarioId is null)
    {
        return RedirectToAction("Login", "Account");
    }

    var motorista =
        await _motoristaService.BuscarPorUsuarioIdAsync(usuarioId.Value);

    if (motorista is null)
    {
        return RedirectToAction(nameof(DadosPessoais));
    }

    if (motorista.Status != StatusCadastro.CorrecaoSolicitada)
    {
        return RedirectToAction(nameof(Painel));
    }

    var dadosPessoais = new CadastroMotoristaViewModel
    {
        NomeCompleto = motorista.NomeCompleto,
        Cpf = motorista.Cpf,
        DataNascimento = motorista.DataNascimento,
        TelefonePrincipal = motorista.TelefonePrincipal,
        TelefoneSecundario = motorista.TelefoneSecundario,
        NumeroCnh = motorista.NumeroCnh,
        CategoriaCnh = motorista.CategoriaCnh,
        PossuiCertificacaoEspecial =
            motorista.PossuiCertificacaoEspecial,
        TipoCertificacao =
            motorista.TipoCertificacao
    };

    var endereco = new EnderecoViewModel
    {
        Cep = motorista.Endereco.Cep,
        RuaAvenida = motorista.Endereco.RuaAvenida,
        Numero = motorista.Endereco.Numero,
        Complemento = motorista.Endereco.Complemento,
        Bairro = motorista.Endereco.Bairro,
        Cidade = motorista.Endereco.Cidade,
        Estado = motorista.Endereco.Estado
    };

    var veiculo = new VeiculoViewModel
    {
        Placa = motorista.Veiculo.Placa,
        Ano = motorista.Veiculo.Ano,
        Marca = motorista.Veiculo.Marca,
        Modelo = motorista.Veiculo.Modelo,
        TipoVeiculo = motorista.Veiculo.TipoVeiculo,
        PossuiRastreador =
            motorista.Veiculo.PossuiRastreador,
        TipoRastreador =
            motorista.Veiculo.TipoRastreador,
        EmpresaRastreador =
            motorista.Veiculo.EmpresaRastreador
    };

    SalvarNaSession(
        DadosPessoaisSessionKey,
        dadosPessoais
    );

    SalvarNaSession(
        EnderecoSessionKey,
        endereco
    );

    SalvarNaSession(
        VeiculoSessionKey,
        veiculo
    );

    return RedirectToAction(nameof(DadosPessoais));
}

    // =====================================================
    // MÉTODOS AUXILIARES
    // =====================================================

    private int? ObterUsuarioId()
    {
        var idClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idClaim, out var usuarioId))
        {
            return null;
        }

        return usuarioId;
    }

    private void SalvarNaSession<T>(string chave, T valor)
    {
        var json = JsonSerializer.Serialize(valor);

        HttpContext.Session.SetString(chave, json);
    }

    private T? ObterDaSession<T>(string chave)
    {
        var json = HttpContext.Session.GetString(chave);

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json);
    }

    private bool ExisteNaSession(string chave)
    {
        return HttpContext.Session.GetString(chave) is not null;
    }

    private void LimparCadastroDaSession()
    {
        HttpContext.Session.Remove(DadosPessoaisSessionKey);
        HttpContext.Session.Remove(EnderecoSessionKey);
        HttpContext.Session.Remove(VeiculoSessionKey);
    }

    
}