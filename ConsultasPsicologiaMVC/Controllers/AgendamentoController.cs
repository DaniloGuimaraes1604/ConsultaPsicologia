using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ConsultasPsicologiaMVC.ENUMS;
using ConsultasPsicologiaMVC.BLL.Interfaces;

namespace ConsultasPsicologiaMVC.Controllers
{
    [Authorize]
    public class AgendamentoController : Controller
    {
        private readonly IAgendamentoBll _agendamentobll;

        public AgendamentoController(IAgendamentoBll agendamentoBll)
        {
            _agendamentobll = agendamentoBll;
        }

        [HttpPost]
        public IActionResult SalvarAgendamento([FromBody] AgendamentoDto agendamentoDto)
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

                        var idPaciente = _agendamentobll.ConsultaIdPaciente(agendamento.PacienteId);

                        if (idPaciente.Ativo == 0)
                        {
                            return Json(new { success = false, message = "Usuário bloqueado para agendamento de consultas, entre em contato com a clinica." });
                        }

                        if (_agendamentobll.SalvarAgendamento(agendamento, idPaciente.Id))
                        {
                            return Json(new { success = true, message = "Consulta agendada com sucesso!" });
                        }
                        else 
                        {
                            return Json(new { success = false, message = "Erro ao salvar a consulta, por favor tente novamente!" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Formato de data ou hora inválido." });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Ocorreu um erro ao salvar a consulta: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Dados inválidos." });
        }
    }
}
