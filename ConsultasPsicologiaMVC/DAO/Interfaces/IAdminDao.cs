using ConsultasPsicologiaMVC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.DAO.Interfaces
{
    public interface IAdminDao
    {
        Task<List<Cadastrar>> ListarPacientes();
        Task<Cadastrar> BuscarPacientePorId(int id);
        Task<bool> AtualizarPaciente(Cadastrar paciente);
        Task<bool> ExcluirPaciente(int id);
        Task<Cadastrar> BuscarPacientePorEmail(string email);
        (List<PacienteViewModel> pacientes, int totalCount) GetFilteredAndPagedPacientes(
            int page,
            int pageSize,
            string nomeCompleto,
            string nomeCompletoType,
            string dataNascimento,
            string dataNascimentoType,
            string email,
            string emailType
        );
    }
}