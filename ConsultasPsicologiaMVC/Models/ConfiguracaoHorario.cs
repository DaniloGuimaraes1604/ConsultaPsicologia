using System;
using System.ComponentModel.DataAnnotations;

namespace ConsultasPsicologiaMVC.Models
{
    public class ConfiguracaoHorario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan HoraInicioAtendimento { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan HoraFimAtendimento { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan HoraInicioAlmoco { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan HoraFimAlmoco { get; set; }

        // Armazena os dias ativos como uma string separada por vírgulas (ex: "1,2,3,4,5" para seg-sex)
        [Required]
        public string DiasAtivos { get; set; } = ""; 
    }
}
