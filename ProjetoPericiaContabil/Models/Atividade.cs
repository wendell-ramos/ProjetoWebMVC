using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPericiaContabil.Models
{
    [Table("Atividades")]
    public class Atividade
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 100 caracteres.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "A descrição deve ter entre 5 e 1000 caracteres.")]
        public string Descricao { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "EmAnalise";

        [Required]
        public int ClienteId { get; set; }

        public int? FuncionarioId { get; set; }

        [Required(ErrorMessage = "O tipo de cálculo é obrigatório.")]
        [StringLength(50)]
        public string TipoCalculo { get; set; }

        [StringLength(3000)]
        public string Resultado { get; set; }

        public bool ClienteVisualizou { get; set; } = false;

        [StringLength(255)]
        public string ArquivoClienteNome { get; set; }

        [StringLength(500)]
        public string ArquivoClienteCaminho { get; set; }

        [StringLength(255)]
        public string ArquivoResultadoNome { get; set; }

        [StringLength(500)]
        public string ArquivoResultadoCaminho { get; set; }
    }
}
