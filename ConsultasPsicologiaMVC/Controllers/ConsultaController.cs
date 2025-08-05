using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultasPsicologiaMVC.Controllers
{
    [Authorize] // Apenas usuários autenticados podem acessar
    public class ConsultaController : Controller
    {
        // GET: Consulta
        public IActionResult Index()
        {
            // Aqui você pode adicionar lógica para buscar dados de consultas
            // e passá-los para a View, se necessário.
            return View();
        }
    }
}
