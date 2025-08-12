namespace ConsultasPsicologiaMVC.Models
{
    public class PacienteViewModel
    {
        public int Id { get; set; }
        public string? NomeCompleto { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Email { get; set; }
    }
}
