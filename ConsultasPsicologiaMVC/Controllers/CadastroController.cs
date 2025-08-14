using ConsultasPsicologiaMVC.BLL.Interfaces;
using ConsultasPsicologiaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.Controllers
{
    public class CadastroController : Controller
    {
        private readonly ICadastroBll _cadastroBll;
        private readonly ILogger<CadastroController> _logger;

        public CadastroController(ICadastroBll cadastroBll, ILogger<CadastroController> logger)
        {
            _cadastroBll = cadastroBll;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Salvar([FromBody] Cadastrar model)
        {
            _logger.LogInformation("Método Salvar iniciado.");
            _logger.LogInformation("Dados recebidos: Nome={Nome}, Email={Email}, DataNascimento={DataNascimento}", model.Nome, model.Email, model.DataNascimento);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState inválido. Erros: {Errors}", string.Join(", ", errors));
                return Json(new { success = false, message = "Dados inválidos: " + string.Join(", ", errors) });
            }

            try
            {
                var resultado = _cadastroBll.Salvar(model);

                if (resultado == "Cadastro realizado com sucesso!")
                {
                    _logger.LogInformation("Cadastro realizado com sucesso para o email: {Email}", model.Email);
                    return Json(new { success = true, message = resultado });
                }
                else
                {
                    _logger.LogWarning("Ocorreu um erro ao salvar o cadastro para o email: {Email}. Motivo: {Motivo}", model.Email, resultado);
                    return Json(new { success = false, message = resultado });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma exceção ao salvar o cadastro. InnerException: {InnerException}", ex.InnerException?.Message);

                string logFilePath = Path.Combine(@"C:\Users\danilo.guimaraes\Desktop\Dan\Projeto Piloto Consultas\ConsultasPsicologiaMVC", "logerroraiz.txt");
                string logMessage = $"[{DateTime.Now}] Erro ao salvar cadastro: {ex.Message}\nInnerException: {ex.InnerException?.Message}\nStackTrace: {ex.StackTrace}\n\n";
                System.IO.File.AppendAllText(logFilePath, logMessage);

                return Json(new { success = false, message = "Ocorreu um erro crítico ao salvar o cadastro." });
            }
        }
    }
}