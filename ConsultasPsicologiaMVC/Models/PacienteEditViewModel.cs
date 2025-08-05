using System.ComponentModel.DataAnnotations;

namespace ConsultasPsicologiaMVC.Models
{
    public class PacienteEditViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string NomeCompleto { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        // Campos mockados
        public string Observacao { get; set; }
        public int QuantidadeConsultas { get; set; }
        public double NotaPaciente { get; set; }
        public bool PacienteBloqueado { get; set; }
    }
}
