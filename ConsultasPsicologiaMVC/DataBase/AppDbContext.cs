using ConsultasPsicologiaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultasPsicologiaMVC.DataBase
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> inject) : base (inject)
        {
        }

        
        public DbSet<Agendamento> Agendamentos { get; set; }
        
    }
}
