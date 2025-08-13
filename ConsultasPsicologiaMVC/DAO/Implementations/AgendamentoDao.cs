using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using System.Data;

namespace ConsultasPsicologiaMVC.DAO.Implementations
{
    public class AgendamentoDao : IAgendamentoDao
    {
        private readonly IDbConnection _connection;

        public AgendamentoDao(IDbConnection connection)
        {
            _connection = connection;
        }

        public int SalvarAgendamento(Agendamento dadosAgendamento)
        {
            // O usuário implementará o corpo do método aqui.
            // Por enquanto, um retorno de placeholder.
            return 0;
        }
    }
}