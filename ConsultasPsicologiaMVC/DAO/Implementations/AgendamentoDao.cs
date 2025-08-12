using ConsultasPsicologiaMVC.DataBase;
using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ConsultasPsicologiaMVC.DAO.Implementations
{
    public class AgendamentoDao : IAgendamentoDao
    {
        private readonly AppDbContext _context;

        public AgendamentoDao(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SalvarAgendamento(Agendamento agendamento)
        {
            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();
            return true; // Or handle specific success/failure based on SaveChangesAsync result
        }
    }
}