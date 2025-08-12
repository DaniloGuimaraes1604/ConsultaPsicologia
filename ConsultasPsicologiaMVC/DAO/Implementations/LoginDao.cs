using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using System;
using System.Security.Cryptography;

namespace ConsultasPsicologiaMVC.DAO.Implementations
{
    public class LoginDao : ILoginDao
    {
        private readonly IDbConnection _connection;

        public LoginDao(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Cadastrar> BuscarUsuarioPorEmail(string email)
        {
            Cadastrar user = null;
            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                await connection.OpenAsync();
                // Corrected SQL to only select existing columns
                var sql = "SELECT id, nome, email, senha, datanascimento, ativo FROM usuariopaciente WHERE email = @email";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("email", email.ToUpper());
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new Cadastrar
                            {
                                Id = reader.GetInt32(0),
                                Nome = reader.GetString(1),
                                Email = reader.GetString(2),
                                Senha = reader.GetString(3),
                                DataNascimento = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Ativo = reader.GetInt32(5) == 1 // Corrected index
                            };
                        }
                    }
                }
            }
            return user;
        }

        public async Task<Cadastrar> ValidarLogin(string email, string senha)
        {
            var user = await BuscarUsuarioPorEmail(email);

            if (user == null)
            {
                return null;
            }

            string storedPasswordHash = user.Senha;

            // Extrair salt e hash da senha armazenada
            byte[] hashBytes = Convert.FromBase64String(storedPasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Hashear a senha fornecida com o salt extraído
            var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, 10000, HashAlgorithmName.SHA256);
            byte[] providedPasswordHash = pbkdf2.GetBytes(20);

            // Comparar os hashes
            bool passwordMatch = true;
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != providedPasswordHash[i])
                {
                    passwordMatch = false;
                    break;
                }
            }

            return passwordMatch ? user : null;
        }
    }
}