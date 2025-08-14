
using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using System;

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
            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                await connection.OpenAsync();
                var sql = "INSERT INTO usuariopaciente (nome, email, senha, datanascimento, ativo) VALUES (@nome, @email, @senha, @datanascimento, @ativo)";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("nome", model.Nome));
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("email", model.Email));
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("senha", model.Senha));
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("datanascimento", model.DataNascimento.HasValue ? (object)model.DataNascimento.Value : DBNull.Value));
                    command.Parameters.Add(new Npgsql.NpgsqlParameter("ativo", model.Ativo ? 1 : 0));

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
