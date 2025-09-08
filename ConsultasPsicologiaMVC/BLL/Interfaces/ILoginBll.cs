
using ConsultasPsicologiaMVC.Models;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.BLL.Interfaces
{
    public interface ILoginBll
    {
        Task<LoginResult> ValidarLogin(LoginModel model);
    }
}
