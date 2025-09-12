using ConsultasPsicologiaMVC.BLL.Interfaces;
using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IAdminDao _adminDao;
        private readonly IAdminBll _adminBll;

        public AdminController(IAdminDao adminDao, IAdminBll adminBll)
        {
            _adminDao = adminDao;
            _adminBll = adminBll;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPacientes(
            int page = 1,
            int pageSize = 15,
            string? nomeCompleto = null,
            string? nomeCompletoType = null,
            string? dataNascimento = null,
            string? dataNascimentoType = null,
            string? email = null,
            string? emailType = null)
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
            if (User.Identity?.IsAuthenticated != true || User.FindFirst(ClaimTypes.Email)?.Value.ToLower() != "caroline.adm@adm.com")
            {
                return Forbid();
            }

            var model = await _adminBll.GetPacienteParaEdicao(email);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PacienteEditViewModel model)
        {
            if (User.Identity?.IsAuthenticated != true || User.FindFirst(ClaimTypes.Email)?.Value.ToLower() != "caroline.adm@adm.com")
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var success = await _adminBll.AtualizarPaciente(model);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Dados do paciente {model.NomeCompleto} atualizados com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Não foi possível atualizar os dados do paciente.";
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult GetConsultas(
            int page = 1,
            int pageSize = 15,
            string? nomeCompleto = null,
            string? nomeCompletoType = null,
            string? dataNascimento = null,
            string? dataNascimentoType = null,
            string? email = null,
            string? emailType = null)
        {
            var (pacientes, totalCount) = _adminDao.GetFilteredAndPagedConsultas(
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

            return Json(new
            {
                html = RenderPartialViewToString("_ConsultaTableRows", pacientes),
                totalPages = totalPages
            });
        }
    }
}