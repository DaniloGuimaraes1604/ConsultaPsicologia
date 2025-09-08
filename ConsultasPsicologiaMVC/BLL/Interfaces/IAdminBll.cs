
using ConsultasPsicologiaMVC.Models;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.BLL.Interfaces
{
    public interface IAdminBll
    {
        Task<PacienteEditViewModel> GetPacienteParaEdicao(string email);
        Task<bool> AtualizarPaciente(PacienteEditViewModel model);
    }
}
