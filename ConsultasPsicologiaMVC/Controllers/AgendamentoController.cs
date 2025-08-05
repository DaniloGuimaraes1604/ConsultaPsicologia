using ConsultasPsicologiaMVC.DataBase;
using ConsultasPsicologiaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ConsultasPsicologiaMVC.Controllers
{
    [Authorize]
    public class AgendamentoController : Controller
    {
        private readonly AppDbContext _context;

        public AgendamentoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Salvar([FromBody] AgendamentoDto agendamentoDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var agendamento = new Agendamento
                    {
                        DataHora = agendamentoDto.DataHora
                    };

                    // Futuramente, podemos adicionar validações, como verificar se o horário já está ocupado.
                    _context.Agendamentos.Add(agendamento);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Consulta agendada com sucesso!" });
                }
                catch (Exception ex)
                {
                    // Log do erro (importante para depuração)
                    return Json(new { success = false, message = "Ocorreu um erro ao salvar a consulta: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Dados inválidos." });
        }
    }
}
