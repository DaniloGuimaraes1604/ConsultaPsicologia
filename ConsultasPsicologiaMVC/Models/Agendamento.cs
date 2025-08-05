using System.ComponentModel.DataAnnotations;

namespace ConsultasPsicologiaMVC.Models
{
    public class Agendamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? TipoConsulta { get; set; }

        // Futuramente, podemos adicionar mais detalhes, como:
        // public int PacienteId { get; set; }
        // public string Status { get; set; } = "Confirmado";
    }
}
