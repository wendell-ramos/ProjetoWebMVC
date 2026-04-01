using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetoPericiaContabil.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }

        public string Tipo { get; set; } // Cliente, Funcionario, Admin
        public string Cargo { get; set; } // Civel, Trabalhista, etc (só funcionario)
    }
}