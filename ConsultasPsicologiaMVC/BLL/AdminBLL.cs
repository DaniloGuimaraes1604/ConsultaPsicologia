
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

            var qtdConsultaPac = _adminDao.QuantidadeDeConsultaPaciente(paciente.Id);

            var model = new PacienteEditViewModel
            {
                Email = paciente.Email,
                NomeCompleto = paciente.Nome,
                DataNascimento = paciente.DataNascimento,
                Observacao = "Esta é uma observação mockada para o paciente.", 
                QuantidadeConsultas = qtdConsultaPac,
                NotaPaciente = 4.5, 
                PacienteBloqueado = !paciente.Ativo
            };

            return model;
        }

        public async Task<bool> AtualizarPaciente(PacienteEditViewModel model)
        {
            var paciente = await _adminDao.BuscarPacientePorEmail(model.Email);
            if (paciente == null)
            {
                return false; 
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
