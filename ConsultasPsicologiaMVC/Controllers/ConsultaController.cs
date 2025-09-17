using ConsultasPsicologiaMVC.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultasPsicologiaMVC.Controllers
{
    [Authorize] 
    public class ConsultaController : Controller
    {
        private readonly IAdminBll _adminBll;

        public ConsultaController(IAdminBll adminBll)
        {
            _adminBll = adminBll;
        }

        public IActionResult Index()
        {
            const int pageSize = 10;
            const int page = 1;

            // Busca a primeira página de dados para o carregamento inicial
            var result = _adminBll.RegistroConsultas(page, pageSize, null, null, null, null, null, null, null, null, null, null);

            // Passa os dados de paginação para a View para o JavaScript poder construir os controles
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)result.totalCount / pageSize);
            ViewBag.CurrentPage = page;

            // Passa a lista de consultas como o modelo da View
            return View(result.consultas);
        }
    }
}