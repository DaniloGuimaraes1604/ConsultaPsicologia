using ConsultasPsicologiaMVC.ENUMS;

namespace ConsultasPsicologiaMVC.Models
{
    public class AgendamentoDto
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string Hora { get; set; }
        public ENUMTIPOCONSULTA TipoConsulta { get; set; }
        public string PacienteId { get; set; }
        public double ValorConsulta { get; set; }
        public int StatusConsulta { get; set; }
    }
}
