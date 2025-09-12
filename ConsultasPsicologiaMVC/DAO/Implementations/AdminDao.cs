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


        public int QuantidadeDeConsultaPaciente(int pacienteId)
        {
            var qtdConsultaPac = 0;

            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                connection.OpenAsync();
                var sql = "select count(ID) from REGISTROCONSULTA where pacienteid = :PACIENTEID";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("PACIENTEID", pacienteId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            qtdConsultaPac = reader.GetInt32(0);
                        }
                    }
                }
            }
            return qtdConsultaPac;
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

        public (List<PacienteViewModel> pacientes, int totalCount) GetFilteredAndPagedPacientes(
            int page,
            int pageSize,
            string nomeCompleto,
            string nomeCompletoType,
            string dataNascimento,
            string dataNascimentoType,
            string email,
            string emailType)
        {
            var pacientes = new List<PacienteViewModel>();
            int totalCount = 0;

            try // Outer try-catch for the entire method
            {
                using (var connection = new NpgsqlConnection(_connection.ConnectionString))
                {
                    connection.Open(); // Consider changing to OpenAsync() if the method becomes async

                    var countSql = "SELECT COUNT(*) FROM usuariopaciente WHERE 1=1";
                    var selectSql = "SELECT id, nome, datanascimento, email FROM usuariopaciente WHERE 1=1";

                    var parameters = new List<NpgsqlParameter>();

                    // Apply filters
                    if (!string.IsNullOrEmpty(nomeCompleto))
                    {
                        if (nomeCompletoType == "equals")
                        {
                            countSql += " AND UPPER(nome) = UPPER(@nomeCompleto)";
                            selectSql += " AND UPPER(nome) = UPPER(@nomeCompleto)";
                            parameters.Add(new NpgsqlParameter("nomeCompleto", nomeCompleto));
                        }
                        else // contains
                        {
                            countSql += " AND UPPER(nome) LIKE UPPER(@nomeCompleto)";
                            selectSql += " AND UPPER(nome) LIKE UPPER(@nomeCompleto)";
                            parameters.Add(new NpgsqlParameter("nomeCompleto", "%" + nomeCompleto + "%"));
                        }
                    }

                    if (!string.IsNullOrEmpty(dataNascimento))
                    {
                        if (DateTime.TryParse(dataNascimento, out DateTime parsedDate))
                        {
                            // For date, we assume 'equals' means the exact date
                            // Consider using date_trunc for more robust date comparison if time component is an issue
                            countSql += " AND datanascimento = @dataNascimento";
                            selectSql += " AND datanascimento = @dataNascimento";
                            parameters.Add(new NpgsqlParameter("dataNascimento", parsedDate));
                        }
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        if (emailType == "equals")
                        {
                            countSql += " AND UPPER(email) = UPPER(@email)";
                            selectSql += " AND UPPER(email) = UPPER(@email)";
                            parameters.Add(new NpgsqlParameter("email", email));
                        }
                        else // contains
                        {
                            countSql += " AND UPPER(email) LIKE UPPER(@email)";
                            selectSql += " AND UPPER(email) LIKE UPPER(@email)";
                            parameters.Add(new NpgsqlParameter("email", "%" + email + "%"));
                        }
                    }

                    Console.WriteLine($"[DEBUG] Count SQL: {countSql}");
                    Console.WriteLine($"[DEBUG] Select SQL: {selectSql}");
                    foreach (var p in parameters)
                    {
                        Console.WriteLine($"[DEBUG] Parameter: {p.ParameterName} = {p.Value}");
                    }

                    // Get total count
                    try
                    {
                        using (var command = new NpgsqlCommand(countSql, connection))
                        {
                            command.Parameters.AddRange(parameters.ToArray());
                            totalCount = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Error getting total count: {ex.Message}");
                        throw; // Re-throw to propagate the error
                    }

                    // Apply pagination
                    selectSql += " ORDER BY nome LIMIT @pageSize OFFSET @offset";
                    
                    // Create a NEW list of parameters for the select command, performing a deep copy of existing parameters
                    var selectParameters = new List<NpgsqlParameter>();
                    foreach (var p in parameters)
                    {
                        selectParameters.Add(new NpgsqlParameter(p.ParameterName, p.Value)); // Deep copy
                    }
                    
                    selectParameters.Add(new NpgsqlParameter("pageSize", pageSize));
                    selectParameters.Add(new NpgsqlParameter("offset", (page - 1) * pageSize));

                    try
                    {
                        using (var command = new NpgsqlCommand(selectSql, connection))
                        {
                            command.Parameters.AddRange(selectParameters.ToArray()); // Use the NEW list
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    pacientes.Add(new PacienteViewModel
                                    {
                                        Id = reader.GetInt32(0),
                                        NomeCompleto = reader.GetString(1),
                                        DataNascimento = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                                        Email = reader.GetString(3)
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Error executing select query: {ex.Message}");
                        throw; // Re-throw to propagate the error
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] General error in GetFilteredAndPagedPacientes: {ex.Message}");
                // Optionally, log inner exception or stack trace
                throw; // Re-throw to propagate the error
            }
            return (pacientes, totalCount);
        }


        public (List<AgendamentoDto> pacientes, int totalCount) GetFilteredAndPagedConsultas(
            int page,
            int pageSize,
            string nomeCompleto,
            string nomeCompletoType,
            string dataNascimento,
            string dataNascimentoType,
            string email,
            string emailType)
        {
            var pacientes = new List<AgendamentoDto>();
            int totalCount = 0;

            try // Outer try-catch for the entire method
            {
                using (var connection = new NpgsqlConnection(_connection.ConnectionString))
                {
                    connection.Open(); // Consider changing to OpenAsync() if the method becomes async

                    var countSql = "SELECT COUNT(*) FROM usuariopaciente WHERE 1=1";
                    var selectSql = "SELECT id, nome, datanascimento, email FROM usuariopaciente WHERE 1=1";

                    var parameters = new List<NpgsqlParameter>();

                    // Apply filters
                    if (!string.IsNullOrEmpty(nomeCompleto))
                    {
                        if (nomeCompletoType == "equals")
                        {
                            countSql += " AND UPPER(nome) = UPPER(@nomeCompleto)";
                            selectSql += " AND UPPER(nome) = UPPER(@nomeCompleto)";
                            parameters.Add(new NpgsqlParameter("nomeCompleto", nomeCompleto));
                        }
                        else // contains
                        {
                            countSql += " AND UPPER(nome) LIKE UPPER(@nomeCompleto)";
                            selectSql += " AND UPPER(nome) LIKE UPPER(@nomeCompleto)";
                            parameters.Add(new NpgsqlParameter("nomeCompleto", "%" + nomeCompleto + "%"));
                        }
                    }

                    if (!string.IsNullOrEmpty(dataNascimento))
                    {
                        if (DateTime.TryParse(dataNascimento, out DateTime parsedDate))
                        {
                            // For date, we assume 'equals' means the exact date
                            // Consider using date_trunc for more robust date comparison if time component is an issue
                            countSql += " AND datanascimento = @dataNascimento";
                            selectSql += " AND datanascimento = @dataNascimento";
                            parameters.Add(new NpgsqlParameter("dataNascimento", parsedDate));
                        }
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        if (emailType == "equals")
                        {
                            countSql += " AND UPPER(email) = UPPER(@email)";
                            selectSql += " AND UPPER(email) = UPPER(@email)";
                            parameters.Add(new NpgsqlParameter("email", email));
                        }
                        else // contains
                        {
                            countSql += " AND UPPER(email) LIKE UPPER(@email)";
                            selectSql += " AND UPPER(email) LIKE UPPER(@email)";
                            parameters.Add(new NpgsqlParameter("email", "%" + email + "%"));
                        }
                    }

                    Console.WriteLine($"[DEBUG] Count SQL: {countSql}");
                    Console.WriteLine($"[DEBUG] Select SQL: {selectSql}");
                    foreach (var p in parameters)
                    {
                        Console.WriteLine($"[DEBUG] Parameter: {p.ParameterName} = {p.Value}");
                    }

                    // Get total count
                    try
                    {
                        using (var command = new NpgsqlCommand(countSql, connection))
                        {
                            command.Parameters.AddRange(parameters.ToArray());
                            totalCount = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Error getting total count: {ex.Message}");
                        throw; // Re-throw to propagate the error
                    }

                    // Apply pagination
                    selectSql += " ORDER BY nome LIMIT @pageSize OFFSET @offset";

                    // Create a NEW list of parameters for the select command, performing a deep copy of existing parameters
                    var selectParameters = new List<NpgsqlParameter>();
                    foreach (var p in parameters)
                    {
                        selectParameters.Add(new NpgsqlParameter(p.ParameterName, p.Value)); // Deep copy
                    }

                    selectParameters.Add(new NpgsqlParameter("pageSize", pageSize));
                    selectParameters.Add(new NpgsqlParameter("offset", (page - 1) * pageSize));

                    try
                    {
                        using (var command = new NpgsqlCommand(selectSql, connection))
                        {
                            command.Parameters.AddRange(selectParameters.ToArray()); // Use the NEW list
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    pacientes.Add(new AgendamentoDto
                                    {
                                        Id = reader.GetInt32(0),
                                        //NomeCompleto = reader.GetString(1),
                                        //PacienteId = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                                        //Email = reader.GetString(3)
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Error executing select query: {ex.Message}");
                        throw; // Re-throw to propagate the error
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] General error in GetFilteredAndPagedPacientes: {ex.Message}");
                // Optionally, log inner exception or stack trace
                throw; // Re-throw to propagate the error
            }
            return (pacientes, totalCount);
        }
    }
}