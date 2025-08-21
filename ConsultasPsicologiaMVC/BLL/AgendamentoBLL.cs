using ConsultasPsicologiaMVC.BLL.Interfaces;
using ConsultasPsicologiaMVC.DAO.Implementations;
using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;

namespace ConsultasPsicologiaMVC.BLL
{
    public class AgendamentoBLL : IAgendamentoBll
    {
        private readonly IAgendamentoDao _agendamentoDao;

        public AgendamentoBLL(IAgendamentoDao agendamentoDao)
        {
            _agendamentoDao = agendamentoDao;
        }

        public bool SalvarAgendamento(Agendamento dadosAgendamento, int idPaciente)
        {
            var consultaAgendada = _agendamentoDao.SalvarAgendamento(dadosAgendamento, idPaciente);

            if (consultaAgendada)
            {
               return true;
            }
            return false;
        }


        public int ConsultaIdPaciente(string emailPaciente) 
        {
            var idPaciente = _agendamentoDao.ConsultaIdPaciente(emailPaciente);
            if (idPaciente <= 0)
            {
                throw new Exception("ID do paciente nao encontrado, por favor tente novamente");
            }

            return idPaciente;
        }
    }
}
