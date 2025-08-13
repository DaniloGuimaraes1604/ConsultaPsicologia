using ConsultasPsicologiaMVC.ENUMS;
using System.ComponentModel.DataAnnotations;

namespace ConsultasPsicologiaMVC.Models
{
    public class Agendamento
    {
        [Key]
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public ENUMTIPOCONSULTA TipoConsulta{ get; set; }
                public string PacienteId { get; set; }
        public double ValorConsulta { get; set; }
        public int StatusConsulta { get; set; }

    }
}
