using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TransVias.Models;
using TransVias.Models.Enums;
using TransVias.Services;
using TransVias.ViewModels;

namespace TransVias.Controllers;

public class AccountController : Controller
{
    private readonly AccountService _accountService;
    private readonly MotoristaService _motoristaService;

    public AccountController(
        AccountService accountService,
        MotoristaService motoristaService)
    {
        _accountService = accountService;
        _motoristaService = motoristaService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(CriarContaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultado = await _accountService.CriarContaAsync(
            model.Email,
            model.Senha,
            model.ConfirmarSenha
        );

        if (resultado.Erro is not null)
        {
            ModelState.AddModelError(string.Empty, resultado.Erro);
            return View(model);
        }

        await CriarSessaoAsync(resultado.Usuario!);

        return RedirectToAction(
            "DadosPessoais",
            "Motorista"
        );
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usuario = await _accountService.AutenticarAsync(
            model.Email,
            model.Senha
        );

        if (usuario is null)
        {
            ModelState.AddModelError(
                string.Empty,
                "E-mail ou senha inválidos."
            );

            return View(model);
        }

        await CriarSessaoAsync(usuario);

        if (usuario.TipoUsuario == TipoUsuario.Administrador)
        {
            return RedirectToAction(
                "Dashboard",
                "Admin"
            );
        }

        var motorista =
            await _motoristaService.BuscarPorUsuarioIdAsync(usuario.Id);

        if (motorista is null)
        {
            return RedirectToAction(
                "DadosPessoais",
                "Motorista"
            );
        }

        return RedirectToAction(
            "Painel",
            "Motorista"
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction(
            "Index",
            "Home"
        );
    }

    private async Task CriarSessaoAsync(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()
            ),

            new Claim(
                ClaimTypes.Email,
                usuario.Email
            ),

            new Claim(
                ClaimTypes.Role,
                usuario.TipoUsuario.ToString()
            )
        };

        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identidade);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );
    }
}