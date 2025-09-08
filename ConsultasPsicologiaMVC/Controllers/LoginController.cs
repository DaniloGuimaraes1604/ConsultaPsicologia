
using ConsultasPsicologiaMVC.BLL.Interfaces;
using ConsultasPsicologiaMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginBll _loginBll;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILoginBll loginBll, ILogger<LoginController> logger)
        {
            _loginBll = loginBll;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Entrar([FromBody] LoginModel model)
        {
            _logger.LogInformation("Tentativa de login para o e-mail: {Email}", model.Email);

            var result = await _loginBll.ValidarLogin(model);

            if (result.Success)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, result.Email),
                    new Claim(ClaimTypes.Name, result.UserName),
                    new Claim(ClaimTypes.Email, result.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                _logger.LogInformation("Login bem-sucedido para o e-mail: {Email}", model.Email);
                return Json(new { success = true, message = result.Message, email = result.Email, userName = result.UserName });
            }
            else
            {
                _logger.LogWarning("Login falhou para o e-mail {Email}: {Message}", model.Email, result.Message);
                return Json(new { success = false, message = result.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Usuário deslogado.");
            return Json(new { success = true });
        }
    }
}
