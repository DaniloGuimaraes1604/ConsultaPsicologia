using ConsultasPsicologiaMVC.Models;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.DAO.Interfaces
{
    public interface IAgendamentoDao
    {
        bool SalvarAgendamento(Agendamento dadosAgendamento, int idPaciente);

        int ConsultaIdPaciente(string email);
    }
}