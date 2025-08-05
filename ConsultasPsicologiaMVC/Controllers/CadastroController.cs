using ConsultasPsicologiaMVC.DataBase;
using ConsultasPsicologiaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace ConsultasPsicologiaMVC.Controllers
{
    public class CadastroController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CadastroController> _logger;

        public CadastroController(AppDbContext context, ILogger<CadastroController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Salvar([FromBody] Cadastrar model)
        {
            _logger.LogInformation("Método Salvar iniciado.");
            _logger.LogInformation("Dados recebidos: Nome={Nome}, Email={Email}, DataNascimento={DataNascimento}", model.Nome, model.Email, model.DataNascimento);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState inválido. Erros: {Errors}", string.Join(", ", errors));
                return Json(new { success = false, message = "Dados inválidos: " + string.Join(", ", errors) });
            }

            try
            {
                // Verificar se o e-mail já existe
                string checkEmailSql = "SELECT COUNT(*) FROM usuariopaciente WHERE Email = @Email";
                int emailCount;
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = checkEmailSql;
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("@Email", model.Email));
                    _context.Database.OpenConnection();
                    emailCount = Convert.ToInt32(await command.ExecuteScalarAsync());
                    _context.Database.CloseConnection();
                }

                if (emailCount > 0)
                {
                    _logger.LogWarning("Tentativa de cadastro com e-mail já existente: {Email}", model.Email);
                    return Json(new { success = false, message = "Este e-mail já está cadastrado." });
                }

                // Gerar um salt aleatório
                byte[] salt = new byte[16];
                RandomNumberGenerator.Fill(salt);

                // Criar o hash da senha usando PBKDF2
                var pbkdf2 = new Rfc2898DeriveBytes(model.Senha, salt, 10000, HashAlgorithmName.SHA256); // Especificando SHA256
                byte[] hash = pbkdf2.GetBytes(20); // 20 bytes para o hash

                // Combinar o salt e o hash para armazenamento
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                string senhaArmazenada = Convert.ToBase64String(hashBytes);

                string sql = "INSERT INTO usuariopaciente (Nome, DataNascimento, Email, Senha, Ativo) VALUES (@Nome, @DataNascimento, @Email, @Senha, @Ativo)";
                
                var parameters = new[] 
                {
                    new Npgsql.NpgsqlParameter("@Nome", model.Nome.ToUpper()),
                    new Npgsql.NpgsqlParameter("@DataNascimento", model.DataNascimento), // Simplificado para lidar com anuláveis
                    new Npgsql.NpgsqlParameter("@Email", model.Email.ToUpper()),
                    new Npgsql.NpgsqlParameter("@Senha", senhaArmazenada), // Senha hasheada e salt combinados
                    new Npgsql.NpgsqlParameter("@Ativo", model.Ativo ? 1 : 0)
                };

                _logger.LogInformation("Parâmetros SQL: @Nome={Nome}, @DataNascimento={DataNascimento}, @Email={Email}, @Ativo={Ativo}", model.Nome, model.DataNascimento, model.Email, model.Ativo);
                _logger.LogInformation("Executando SQL: {Sql}", sql);
                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parameters);
                _logger.LogInformation("Comando SQL executado. Linhas afetadas: {RowsAffected}", rowsAffected);

                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Cadastro realizado com sucesso para o email: {Email}", model.Email);
                    return Json(new { success = true, message = "Cadastro realizado com sucesso!" });
                }
                else
                {
                    _logger.LogWarning("O comando INSERT não afetou nenhuma linha para o email: {Email}", model.Email);
                    return Json(new { success = false, message = "Não foi possível realizar o cadastro. Nenhum dado foi salvo." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma exceção ao salvar o cadastro. InnerException: {InnerException}", ex.InnerException?.Message);
                
                string logFilePath = Path.Combine(@"C:\Users\danilo.guimaraes\Desktop\Dan\Projeto Piloto Consultas\ConsultasPsicologiaMVC", "logerroraiz.txt");
                string logMessage = $"[{DateTime.Now}] Erro ao salvar cadastro: {ex.Message}\nInnerException: {ex.InnerException?.Message}\nStackTrace: {ex.StackTrace}\n\n";
                System.IO.File.AppendAllText(logFilePath, logMessage);

                return Json(new { success = false, message = "Ocorreu um erro crítico ao salvar o cadastro." });
            }
        }
    }
}
