using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using Npgsql;
using System.Data;

namespace ConsultasPsicologiaMVC.DAO.Implementations
{
    public class AgendamentoDao : IAgendamentoDao
    {
        private readonly IDbConnection _connection;

        public AgendamentoDao(IDbConnection connection)
        {
            _connection = connection;
        }

        public PacienteDto ConsultaIdPaciente(string emailPaciente)
        {
            PacienteDto paciente = new();

            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                connection.Open();
                var sql = $"select ID, ATIVO from USUARIOPACIENTE where email = :EMAIL";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.Add("EMAIL", NpgsqlTypes.NpgsqlDbType.Varchar, 255).Value = emailPaciente;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                           paciente.Id = reader.GetInt32(0);
                           paciente.Ativo = reader.GetInt32(1);
                        }
                    }
                }
                return paciente;
            }
        }

        public bool SalvarAgendamento(Agendamento dadosAgendamento, int idPaciente)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connection.ConnectionString))
                {
                    connection.Open();

                    var sql = @"
                                    INSERT INTO REGISTROCONSULTA 
                                (TIPOCONSULTAID, PACIENTEID, DATAHORACONSULTA, HORACONSULTA, STATUSCONSULTA, DATAINCLUSAO, DATAALTERACAO) 
                                    VALUES 
                                (@TIPOCONSULTAID, @PACIENTEID, @DATAHORACONSULTA, @HORACONSULTA, @STATUSCONSULTA, @DATAINCLUSAO, @DATAALTERACAO)
                                RETURNING ID;";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@TIPOCONSULTAID", NpgsqlTypes.NpgsqlDbType.Integer).Value = (int)dadosAgendamento.TipoConsulta;
                        command.Parameters.Add("@PACIENTEID", NpgsqlTypes.NpgsqlDbType.Integer).Value = idPaciente;
                        command.Parameters.Add("@DATAHORACONSULTA", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = dadosAgendamento.DataHora;
                        command.Parameters.Add("@HORACONSULTA", NpgsqlTypes.NpgsqlDbType.Time).Value = dadosAgendamento.DataHora.TimeOfDay;
                        command.Parameters.Add("@STATUSCONSULTA", NpgsqlTypes.NpgsqlDbType.Smallint).Value = dadosAgendamento.StatusConsulta;
                        command.Parameters.Add("@DATAINCLUSAO", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now;
                        command.Parameters.Add("@DATAALTERACAO", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DBNull.Value;

                        var insertedId = command.ExecuteScalar();

                        return insertedId != null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao salvar agendamento: " + ex.Message);
                return false;
            }
        }

    }
}