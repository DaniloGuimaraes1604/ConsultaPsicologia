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

        public int ConsultaIdPaciente(string emailPaciente)
        {
            int pacienteId = 0;
            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                connection.Open();
                var sql = "select ID from USUARIOPACIENTE where email = EMAIL";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.Add("EMAIL", NpgsqlTypes.NpgsqlDbType.Varchar, 255).Value = emailPaciente;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                           pacienteId = reader.GetInt32(0);
                        }
                    }
                }
                return pacienteId;
            }
        }

        public int SalvarAgendamento(Agendamento dadosAgendamento, int idPaciente)
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
                        return insertedId != null ? Convert.ToInt32(insertedId) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao salvar agendamento: " + ex.Message);
                return 0;
            }
        }

    }
}