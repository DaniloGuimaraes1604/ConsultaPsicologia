using ConsultasPsicologiaMVC.DataBase;
using ConsultasPsicologiaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Verifica se o usuário logado é o master
            if (User.Identity?.IsAuthenticated == true && User.Identity?.Name == "caroline.adm@adm.com")
            {
                var pacientes = new List<PacienteViewModel>();
                var connectionString = _context.Database.GetDbConnection().ConnectionString;

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "SELECT nome AS NomeCompleto, datanascimento, email FROM usuariopaciente";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                pacientes.Add(new PacienteViewModel
                                {
                                    NomeCompleto = reader.GetString(0),
                                    DataNascimento = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1),
                                    Email = reader.GetString(2)
                                });
                            }
                        }
                    }
                }
                return View(pacientes);
            }
            return Forbid(); // Acesso negado para usuários não-master
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string email)
        {
            if (User.Identity?.IsAuthenticated == true && User.Identity?.Name == "caroline.adm@adm.com")
            {
                // Busca os dados do paciente (apenas para preencher o email e nome/data de nascimento)
                PacienteViewModel paciente = null;
                var connectionString = _context.Database.GetDbConnection().ConnectionString;

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "SELECT nome, datanascimento, email FROM usuariopaciente WHERE email = @email";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("email", email);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                paciente = new PacienteViewModel
                                {
                                    NomeCompleto = reader.GetString(0),
                                    DataNascimento = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1),
                                    Email = reader.GetString(2)
                                };
                            }
                        }
                    }
                }

                if (paciente == null)
                {
                    return NotFound(); // Paciente não encontrado
                }

                // Preenche o PacienteEditViewModel com os dados existentes e mockados
                var model = new PacienteEditViewModel
                {
                    Email = paciente.Email,
                    NomeCompleto = paciente.NomeCompleto,
                    DataNascimento = paciente.DataNascimento,
                    Observacao = "Esta é uma observação mockada para o paciente.",
                    QuantidadeConsultas = 5,
                    NotaPaciente = 4.5,
                    PacienteBloqueado = false
                };
                return View(model);
            }
            return Forbid();
        }

        [HttpPost]
        public IActionResult Edit(PacienteEditViewModel model)
        {
            if (User.Identity?.IsAuthenticated == true && User.Identity?.Name == "caroline.adm@adm.com")
            {
                if (ModelState.IsValid)
                {
                    // Por enquanto, apenas para demonstração, não há persistência real.
                    // Em uma implementação real, você atualizaria o banco de dados aqui.
                    TempData["SuccessMessage"] = $"Dados do paciente {model.NomeCompleto} (Email: {model.Email}) atualizados com sucesso (mock)!\nObservação: {model.Observacao}\nConsultas: {model.QuantidadeConsultas}\nNota: {model.NotaPaciente}\nBloqueado: {model.PacienteBloqueado}";
                    return RedirectToAction("Index");
                }
                // Se o modelo não for válido, retorna a view com os erros
                return View(model);
            }
            return Forbid();
        }
    }
}