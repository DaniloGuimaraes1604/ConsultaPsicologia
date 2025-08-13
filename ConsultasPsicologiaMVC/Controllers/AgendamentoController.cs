using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ConsultasPsicologiaMVC.ENUMS;

namespace ConsultasPsicologiaMVC.Controllers
{
    [Authorize]
    public class AgendamentoController : Controller
    {
        private readonly IAgendamentoDao _agendamentoDao;

        public AgendamentoController(IAgendamentoDao agendamentoDao)
        {
            _agendamentoDao = agendamentoDao;
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] AgendamentoDto agendamentoDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (DateTime.TryParseExact(agendamentoDto.Data + " " + agendamentoDto.Hora, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dataHora))
                    {
                        var agendamento = new Agendamento
                        {
                            DataHora = dataHora,
                            TipoConsulta = agendamentoDto.TipoConsulta,
                            PacienteId = agendamentoDto.PacienteId,
                            ValorConsulta = agendamentoDto.ValorConsulta,
                            StatusConsulta = agendamentoDto.StatusConsulta
                        };

                        _agendamentoDao.SalvarAgendamento(agendamento);
                        return Json(new { success = true, message = "Consulta agendada com sucesso!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Formato de data ou hora inválido." });
                    }
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
