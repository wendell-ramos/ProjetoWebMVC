using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetoPericiaContabil.Models
{
    public class Atividade
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }

        public string Status { get; set; } = "EmAnalise";

        public int ClienteId { get; set; }
        public int? FuncionarioId { get; set; }

        public string TipoCalculo { get; set; }
        // Civel, Trabalhista, Tributario, Previdenciario
        public string Resultado { get; set; }
    }
}