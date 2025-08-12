using ConsultasPsicologiaMVC.Models;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.DAO.Interfaces
{
    public interface ILoginDao
    {
        Task<Cadastrar> BuscarUsuarioPorEmail(string email);
        Task<Cadastrar> ValidarLogin(string email, string senha);
    }
}