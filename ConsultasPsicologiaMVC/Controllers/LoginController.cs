using ConsultasPsicologiaMVC.Models;
using ConsultasPsicologiaMVC.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ConsultasPsicologiaMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AppDbContext context, ILogger<LoginController> logger)
        {
            _context = context;
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

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Senha))
            {
                return Json(new { success = false, message = "E-mail e senha são obrigatórios." });
            }

            try
            {
                string emailUpper = model.Email.ToUpper();
                string sql = "SELECT Senha, Nome FROM usuariopaciente WHERE Email = @Email";
                var emailParam = new Npgsql.NpgsqlParameter("@Email", emailUpper);

                string? storedPasswordHash = null;
                string? userName = null;
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add(emailParam);
                    _context.Database.OpenConnection();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            storedPasswordHash = reader.GetString(0);
                            userName = reader.GetString(1);
                        }
                    }
                    _context.Database.CloseConnection();
                }

                if (string.IsNullOrEmpty(storedPasswordHash))
                {
                    _logger.LogWarning("Login falhou: E-mail {Email} não encontrado.", model.Email);
                    return Json(new { success = false, message = "E-mail ou senha inválidos." });
                }

                // Extrair salt e hash da senha armazenada
                byte[] hashBytes = Convert.FromBase64String(storedPasswordHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                // Hashear a senha fornecida com o salt extraído
                var pbkdf2 = new Rfc2898DeriveBytes(model.Senha, salt, 10000, HashAlgorithmName.SHA256);
                byte[] providedPasswordHash = pbkdf2.GetBytes(20);

                // Comparar os hashes
                bool passwordMatch = true;
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != providedPasswordHash[i])
                    {
                        passwordMatch = false;
                        break;
                    }
                }

                if (passwordMatch)
                {
                    // Autenticação bem-sucedida
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, model.Email), // Adicionado para identificação única
                        new Claim(ClaimTypes.Name, model.Email!),
                        new Claim(ClaimTypes.Email, model.Email!)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // O cookie persistirá entre as sessões do navegador
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // Expira em 7 dias
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    _logger.LogInformation("Login bem-sucedido para o e-mail: {Email}", model.Email);
                    return Json(new { success = true, message = "Login realizado com sucesso!", email = model.Email, userName = userName });
                }
                else
                {
                    _logger.LogWarning("Login falhou: Senha inválida para o e-mail {Email}.", model.Email);
                    return Json(new { success = false, message = "E-mail ou senha inválidos." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar fazer login para o e-mail: {Email}. InnerException: {InnerException}", model.Email, ex.InnerException?.Message);
                                return Json(new { success = false, message = "Ocorreu um erro ao tentar fazer login." });
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