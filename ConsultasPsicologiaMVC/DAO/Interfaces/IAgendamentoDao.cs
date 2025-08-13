using ConsultasPsicologiaMVC.Models;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.DAO.Interfaces
{
    public interface IAgendamentoDao
    {
        int SalvarAgendamento(Agendamento dadosAgendamento);
    }
}