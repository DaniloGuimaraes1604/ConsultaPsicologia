using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using System;
using System.Security.Cryptography;

namespace ConsultasPsicologiaMVC.DAO.Implementations
{
    public class CadastroDao : ICadastroDao
    {
        private readonly IDbConnection _connection;

        public CadastroDao(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> EmailExiste(string email)
        {
            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                await connection.OpenAsync();
                var sql = "SELECT COUNT(*) FROM usuariopaciente WHERE email = @email";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("email", email.ToUpper());
                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }

        public async Task<bool> SalvarCadastro(Cadastrar model)
        {
            // Gerar um salt aleatório
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            // Criar o hash da senha usando PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(model.Senha, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combinar o salt e o hash para armazenamento
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string senhaArmazenada = Convert.ToBase64String(hashBytes);

            model.Senha = senhaArmazenada; // Atribui a senha hasheada ao modelo
            model.Nome = model.Nome.ToUpper();
            model.Email = model.Email.ToUpper();

            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                await connection.OpenAsync();
                // Corrected SQL to only insert into existing columns
                var sql = "INSERT INTO usuariopaciente (nome, email, senha, datanascimento, ativo) VALUES (@nome, @email, @senha, @datanascimento, @ativo)";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("nome", model.Nome));
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("email", model.Email));
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("senha", model.Senha));
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("datanascimento", model.DataNascimento.HasValue ? (object)model.DataNascimento.Value : DBNull.Value));
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("ativo", model.Ativo ? 1 : 0)); // Assuming 'Ativo' is a boolean in model, stored as 1 or 0

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}