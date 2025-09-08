using ConsultasPsicologiaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ConsultasPsicologiaMVC.Controllers
{
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            // Test comment
            HomeModel home = new HomeModel();

            home.Nome = "Danilo";

            return View(home);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
