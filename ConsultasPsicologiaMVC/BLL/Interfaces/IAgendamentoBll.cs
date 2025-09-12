
using ConsultasPsicologiaMVC.Models;

namespace ConsultasPsicologiaMVC.BLL.Interfaces
{
    public interface IAgendamentoBll
    {
        PacienteDto ConsultaIdPaciente(string pacienteId);
        public bool SalvarAgendamento(Agendamento dadosAgendamento, int idPaciente);
    }
}
