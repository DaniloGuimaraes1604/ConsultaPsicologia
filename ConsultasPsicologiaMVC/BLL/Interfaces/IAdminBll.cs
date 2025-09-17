using ConsultasPsicologiaMVC.Models;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.BLL.Interfaces
{
    public interface IAdminBll
    {
        Task<PacienteEditViewModel> GetPacienteParaEdicao(string email);
        Task<bool> AtualizarPaciente(PacienteEditViewModel model);

        (List<AgendamentoDto> consultas, int totalCount) RegistroConsultas(
         int page,
         int pageSize,
         string nomeCompleto,
         string nomeCompletoType,
         string dataConsulta,
         string dataConsultaType,
         string horaConsulta,
         string horaConsultaType,
         string tipoConsulta,
         string tipoConsultaType,
         string statusConsulta,
         string statusConsultaType
         );
    }
}