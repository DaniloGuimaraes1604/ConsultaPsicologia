using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using System;

namespace ConsultasPsicologiaMVC.DAO.Implementations
{
    public class AdminDao : IAdminDao
    {
        private readonly IDbConnection _connection;

        public AdminDao(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Cadastrar>> ListarPacientes()
        {
            var pacientes = new List<Cadastrar>();
            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                await connection.OpenAsync();
                var sql = "SELECT id, nome, datanascimento, email, ativo, senha FROM usuariopaciente";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            pacientes.Add(new Cadastrar
                            {
                                Id = reader.GetInt32(0),
                                Nome = reader.GetString(1),
                                DataNascimento = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                                Email = reader.GetString(3),
                                Ativo = reader.GetInt32(4) == 1,
                                Senha = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return pacientes;
        }

        public async Task<Cadastrar> BuscarPacientePorId(int id)
        {
            Cadastrar paciente = null;
            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                await connection.OpenAsync();
                var sql = "SELECT id, nome, datanascimento, email, ativo, senha FROM usuariopaciente WHERE id = @id";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            paciente = new Cadastrar
                            {
                                Id = reader.GetInt32(0),
                                Nome = reader.GetString(1),
                                DataNascimento = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                                Email = reader.GetString(3),
                                Ativo = reader.GetInt32(4) == 1,
                                Senha = reader.GetString(5)
                            };
                        }
                    }
                }
            }
            return paciente;
        }

        public async Task<bool> AtualizarPaciente(Cadastrar paciente)
        {
            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                await connection.OpenAsync();
                var sql = "UPDATE usuariopaciente SET nome = @nome, datanascimento = @datanascimento, email = @email, ativo = @ativo WHERE id = @id";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("nome", paciente.Nome);
                    command.Parameters.AddWithValue("datanascimento", paciente.DataNascimento.HasValue ? (object)paciente.DataNascimento.Value : DBNull.Value);
                    command.Parameters.AddWithValue("email", paciente.Email);
                    command.Parameters.AddWithValue("ativo", paciente.Ativo ? 1 : 0);
                    command.Parameters.AddWithValue("id", paciente.Id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> ExcluirPaciente(int id)
        {
            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                await connection.OpenAsync();
                var sql = "DELETE FROM usuariopaciente WHERE id = @id";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("id", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<Cadastrar> BuscarPacientePorEmail(string email)
        {
            Cadastrar paciente = null;
            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                await connection.OpenAsync();
                var sql = "SELECT id, nome, datanascimento, email, ativo, senha FROM usuariopaciente WHERE email = @email";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("email", email);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            paciente = new Cadastrar
                            {
                                Id = reader.GetInt32(0),
                                Nome = reader.GetString(1),
                                DataNascimento = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                                Email = reader.GetString(3),
                                Ativo = reader.GetInt32(4) == 1,
                                Senha = reader.GetString(5)
                            };
                        }
                    }
                }
            }
            return paciente;
        }
    }
}