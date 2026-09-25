using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransVias.Models.Enums;
using TransVias.Services;
using TransVias.ViewModels;

namespace TransVias.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly MotoristaService _motoristaService;

    public AdminController(MotoristaService motoristaService)
    {
        _motoristaService = motoristaService;
    }

    // =====================================================
    // DASHBOARD
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var motoristas = await _motoristaService.ListarTodosAsync();

        var resumo = await _motoristaService.ObterResumoDashboardAsync();

        var model = new DashboardAdminViewModel
        {
            Pendentes = resumo.Pendentes,
            Aprovados = resumo.Aprovados,
            Recusados = resumo.Recusados,

            Motoristas = motoristas.Select(m => new MotoristaResumoViewModel
            {
                Id = m.Id,
                NomeCompleto = m.NomeCompleto,

                Marca = m.Veiculo.Marca,
                Modelo = m.Veiculo.Modelo,
                Placa = m.Veiculo.Placa,

                DataEnvio = m.DataEnvio,
                Status = m.Status
            }).ToList()
        };

        return View(model);
    }

    // =====================================================
    // DETALHES DO MOTORISTA
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var motorista = await _motoristaService.BuscarPorIdAsync(id);

        if (motorista is null)
        {
            return NotFound();
        }

        var model = new DetalhesMotoristaViewModel
        {
            Motorista = motorista,
            Observacao = motorista.ObservacaoAnalise ?? string.Empty
        };

        return View("DetalhesMotorista", model);
    }

    // =====================================================
    // APROVAR
    // =====================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Aprovar(int id)
    {
        var resultado = await _motoristaService.AprovarAsync(id);

        if (!resultado.Sucesso)
        {
            TempData["Erro"] = resultado.Erro;

            return RedirectToAction(
                nameof(Detalhes),
                new { id }
            );
        }

        TempData["Sucesso"] = "Cadastro aprovado com sucesso.";

        return RedirectToAction(nameof(Dashboard));
    }

    // =====================================================
    // RECUSAR
    // =====================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Recusar(
        int id,
        string observacao)
    {
        var resultado = await _motoristaService.RecusarAsync(
            id,
            observacao
        );

        if (!resultado.Sucesso)
        {
            TempData["Erro"] = resultado.Erro;

            return RedirectToAction(
                nameof(Detalhes),
                new { id }
            );
        }

        TempData["Sucesso"] = "Cadastro recusado.";

        return RedirectToAction(nameof(Dashboard));
    }

    // =====================================================
    // SOLICITAR CORREÇÃO
    // =====================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SolicitarCorrecao(
        int id,
        string observacao)
    {
        var resultado =
            await _motoristaService.SolicitarCorrecaoAsync(
                id,
                observacao
            );

        if (!resultado.Sucesso)
        {
            TempData["Erro"] = resultado.Erro;

            return RedirectToAction(
                nameof(Detalhes),
                new { id }
            );
        }

        TempData["Sucesso"] =
            "Solicitação de correção enviada ao motorista.";

        return RedirectToAction(nameof(Dashboard));
    }
}