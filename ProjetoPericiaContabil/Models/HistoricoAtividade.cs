using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPericiaContabil.Models
{
    [Table("HistoricosAtividade")]
    public class HistoricoAtividade
    {
        [Key]
        public int Id { get; set; }

        public int? AtividadeId { get; set; }

        public int? UsuarioId { get; set; }

        [StringLength(150)]
        public string NomeUsuario { get; set; }

        [StringLength(50)]
        public string TipoUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Acao { get; set; }

        [Required]
        [StringLength(1000)]
        public string Descricao { get; set; }

        [Required]
        public DateTime DataHora { get; set; }
    }
}
