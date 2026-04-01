using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetoPericiaContabil.Models;
using System.Linq;

namespace ProjetoPericiaContabil.Controllers
{
    public class AtividadeController : Controller
    {
        // GET: Atividades
        public ActionResult Index()
        {
            if (Session["Tipo"] == null)
                return RedirectToAction("Login", "Usuario");

            var lista = db.Atividades.ToList();
            return View(lista);
        }
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Atividade atividade)
        {
            var usuarioId = (int)Session["UsuarioId"];

            atividade.ClienteId = usuarioId;
            atividade.Status = "EmAnalise";

            db.Atividades.Add(atividade);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Pegar(int id)
        {
            var atividade = db.Atividades.Find(id);

            var funcionarioId = (int)Session["UsuarioId"];
            var funcionario = db.Usuarios.Find(funcionarioId);

            if (funcionario.Cargo != atividade.TipoCalculo)
            {
                TempData["Erro"] = "Você não pode pegar essa atividade!";
                return RedirectToAction("Disponiveis");
            }

            atividade.FuncionarioId = funcionario.Id;
            atividade.Status = "EmElaboracao";

            db.SaveChanges();

            return RedirectToAction("Disponiveis");
        }
        public ActionResult Minhas()
        {
            var usuarioId = (int)Session["UsuarioId"];

            var atividades = db.Atividades
                .Where(a => a.ClienteId == usuarioId)
                .ToList();

            return View(atividades);
        }
        public ActionResult Disponiveis()
        {
            if (Session["Tipo"]?.ToString() != "Funcionario")
                return RedirectToAction("Login", "Usuario");

            var atividades = db.Atividades
                .Where(a => a.Status == "EmAnalise")
                .ToList();

            return View(atividades);
        }
        public ActionResult MinhasFuncionario()
        {
            if (Session["Tipo"]?.ToString() != "Funcionario")
                return RedirectToAction("Login", "Usuario");

            var funcionarioId = (int)Session["UsuarioId"];

            var atividades = db.Atividades
                .Where(a => a.FuncionarioId == funcionarioId)
                .ToList();

            return View(atividades);
        }
        public ActionResult Concluir(int id)
        {
            var atividade = db.Atividades.Find(id);

            var funcionarioId = (int)Session["UsuarioId"];

            // segurança (boa prática)
            if (atividade.FuncionarioId != funcionarioId)
            {
                return RedirectToAction("MinhasFuncionario");
            }

            atividade.Status = "Concluida";

            db.SaveChanges();

            return RedirectToAction("MinhasFuncionario");
        }
        public ActionResult Finalizar(int id)
        {
            var atividade = db.Atividades.Find(id);
            return View(atividade);
        }
        [HttpPost]
        public ActionResult Finalizar(Atividade atividade)
        {
            var atividadeDb = db.Atividades.Find(atividade.Id);

            atividadeDb.Resultado = atividade.Resultado;
            atividadeDb.Status = "Concluida";

            db.SaveChanges();

            return RedirectToAction("MinhasFuncionario");
        }
    }
}