using ConsultasPsicologiaMVC.Models;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.DAO.Interfaces
{
    public interface ICadastroDao
    {
        Task<bool> EmailExiste(string email);
        Task<bool> SalvarCadastro(Cadastrar model);
    }
}