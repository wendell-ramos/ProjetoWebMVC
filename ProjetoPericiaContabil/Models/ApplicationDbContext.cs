using ProjetoPericiaContabil.Models;
using System.Collections.Generic;
using System.Data.Entity;

namespace ProjetoPericiaContabil.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Atividade> Atividades { get; set; }
    }
}