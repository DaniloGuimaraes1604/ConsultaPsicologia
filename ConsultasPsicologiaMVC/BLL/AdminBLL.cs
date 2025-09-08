
using ConsultasPsicologiaMVC.BLL.Interfaces;
using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.BLL
{
    public class AdminBLL : IAdminBll
    {
        private readonly IAdminDao _adminDao;

        public AdminBLL(IAdminDao adminDao)
        {
            _adminDao = adminDao;
        }

        public async Task<PacienteEditViewModel> GetPacienteParaEdicao(string email)
        {
            var paciente = await _adminDao.BuscarPacientePorEmail(email);

            if (paciente == null)
            {
                return null;
            }

            // In a real scenario, you would map the properties from the domain model (Cadastrar)
            // to the view model (PacienteEditViewModel). The extra properties would come from other sources.
            var model = new PacienteEditViewModel
            {
                Email = paciente.Email,
                NomeCompleto = paciente.Nome,
                DataNascimento = paciente.DataNascimento,
                Observacao = "Esta é uma observação mockada para o paciente.", // Mocked data
                QuantidadeConsultas = 5, // Mocked data
                NotaPaciente = 4.5, // Mocked data
                PacienteBloqueado = !paciente.Ativo // Example of mapping logic
            };

            return model;
        }

        public async Task<bool> AtualizarPaciente(PacienteEditViewModel model)
        {
            var paciente = await _adminDao.BuscarPacientePorEmail(model.Email);
            if (paciente == null)
            {
                return false; // Or throw an exception
            }

            // Map updated fields from the view model back to the domain model
            paciente.Nome = model.NomeCompleto;
            paciente.DataNascimento = model.DataNascimento;
            paciente.Ativo = !model.PacienteBloqueado;
            // Here you would also update other properties if they were persisted

            return await _adminDao.AtualizarPaciente(paciente);
        }
    }
}
