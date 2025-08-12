using ConsultasPsicologiaMVC.DataBase;
using ConsultasPsicologiaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultasPsicologiaMVC.DAO.Interfaces; // Added
using Microsoft.AspNetCore.Mvc.Rendering; // Added
using Microsoft.AspNetCore.Mvc.ViewEngines; // Added
using Microsoft.AspNetCore.Mvc.ViewFeatures; // Added
using System.IO; // Added
using System; // Added
using Microsoft.Extensions.DependencyInjection; // Added

namespace ConsultasPsicologiaMVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAdminDao _adminDao; // Added

        public AdminController(AppDbContext context, IAdminDao adminDao) // Modified constructor
        {
            _context = context;
            _adminDao = adminDao; // Initialized
        }

        public IActionResult Index()
        {
            // Initial load will be handled by AJAX call from admin.js
            return View();
        }

        [HttpGet]
        public IActionResult GetPacientes(
            int page = 1,
            int pageSize = 15,
            string? nomeCompleto = null, // Made nullable
            string? nomeCompletoType = null, // Made nullable
            string? dataNascimento = null, // Made nullable
            string? dataNascimentoType = null, // Made nullable
            string? email = null, // Made nullable
            string? emailType = null) // Made nullable
        {
            var (pacientes, totalCount) = _adminDao.GetFilteredAndPagedPacientes(
                page,
                pageSize,
                nomeCompleto,
                nomeCompletoType,
                dataNascimento,
                dataNascimentoType,
                email,
                emailType
            );

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return Json(new { 
                html = RenderPartialViewToString("_PacienteTableRows", pacientes),
                totalPages = totalPages
            });
        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;

            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).Wait();
                return writer.GetStringBuilder().ToString();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string email)
        {
            if (User.Identity?.IsAuthenticated == true && User.Identity?.Name == "caroline.adm@adm.com")
            {
                // Busca os dados do paciente (apenas para preencher o email e nome/data de nascimento)
                PacienteViewModel paciente = null;
                var connectionString = _context.Database.GetDbConnection().ConnectionString;

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "SELECT nome, datanascimento, email FROM usuariopaciente WHERE email = @email";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("email", email);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                paciente = new PacienteViewModel
                                {
                                    NomeCompleto = reader.GetString(0),
                                    DataNascimento = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1),
                                    Email = reader.GetString(2)
                                };
                            }
                        }
                    }
                }

                if (paciente == null)
                {
                    return NotFound(); // Paciente não encontrado
                }

                // Preenche o PacienteEditViewModel com os dados existentes e mockados
                var model = new PacienteEditViewModel
                {
                    Email = paciente.Email,
                    NomeCompleto = paciente.NomeCompleto,
                    DataNascimento = paciente.DataNascimento,
                    Observacao = "Esta é uma observação mockada para o paciente.",
                    QuantidadeConsultas = 5,
                    NotaPaciente = 4.5,
                    PacienteBloqueado = false
                };
                return View(model);
            }
            return Forbid();
        }

        [HttpPost]
        public IActionResult Edit(PacienteEditViewModel model)
        {
            if (User.Identity?.IsAuthenticated == true && User.Identity?.Name == "caroline.adm@adm.com")
            {
                if (ModelState.IsValid)
                {
                    // Por enquanto, apenas para demonstração, não há persistência real.
                    // Em uma implementação real, você atualizaria o banco de dados aqui.
                    TempData["SuccessMessage"] = $"Dados do paciente {model.NomeCompleto} (Email: {model.Email}) atualizados com sucesso (mock)!\nObservação: {model.Observacao}\nConsultas: {model.QuantidadeConsultas}\nNota: {model.NotaPaciente}\nBloqueado: {model.PacienteBloqueado}";
                    return RedirectToAction("Index");
                }
                // Se o modelo não for válido, retorna a view com os erros
                return View(model);
            }
            return Forbid();
        }
    }
}