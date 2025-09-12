using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultasPsicologiaMVC.Models
{
    public class Cadastrar
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public DateTime? DataNascimento { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

    }
}
