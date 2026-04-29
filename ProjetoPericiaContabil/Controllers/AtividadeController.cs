using System;
using System.Linq;
using System.Web.Mvc;
using ProjetoPericiaContabil.Models;

namespace ProjetoPericiaContabil.Controllers
{
    public class AtividadeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            try
            {
                if (Session["Tipo"] == null)
                    return RedirectToAction("Login", "Usuario");

                var lista = db.Atividades.ToList();
                return View(lista);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao carregar atividades.";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Atividade atividade)
        {
            try
            {
                var usuarioId = (int)Session["UsuarioId"];

                atividade.ClienteId = usuarioId;
                atividade.Status = "EmAnalise";

                db.Atividades.Add(atividade);
                db.SaveChanges();

                TempData["Sucesso"] = "Atividade criada com sucesso!";
                return RedirectToAction("Minhas");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao criar atividade.";
                return View(atividade);
            }
        }

        public ActionResult Pegar(int id)
        {
            try
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

                TempData["Sucesso"] = "Atividade atribuída com sucesso!";
                return RedirectToAction("Disponiveis");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao pegar atividade.";
                return RedirectToAction("Disponiveis");
            }
        }

        public ActionResult Minhas()
        {
            try
            {
                var usuarioId = (int)Session["UsuarioId"];

                var atividades = db.Atividades
                    .Where(a => a.ClienteId == usuarioId)
                    .ToList();

                return View(atividades);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao carregar suas atividades.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Disponiveis()
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Funcionario")
                    return RedirectToAction("Login", "Usuario");

                var atividades = db.Atividades
                    .Where(a => a.Status == "EmAnalise")
                    .ToList();

                return View(atividades);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao carregar atividades disponíveis.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult MinhasFuncionario()
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Funcionario")
                    return RedirectToAction("Login", "Usuario");

                var funcionarioId = (int)Session["UsuarioId"];

                var atividades = db.Atividades
                    .Where(a => a.FuncionarioId == funcionarioId && a.ClienteVisualizou == false)
                    .ToList();

                return View(atividades);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao carregar suas atividades.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Concluir(int id)
        {
            try
            {
                var atividade = db.Atividades.Find(id);
                var funcionarioId = (int)Session["UsuarioId"];

                if (atividade.FuncionarioId != funcionarioId)
                    return RedirectToAction("MinhasFuncionario");

                atividade.Status = "Concluida";
                db.SaveChanges();

                TempData["Sucesso"] = "Atividade concluída!";
                return RedirectToAction("MinhasFuncionario");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao concluir atividade.";
                return RedirectToAction("MinhasFuncionario");
            }
        }

        public ActionResult Finalizar(int id)
        {
            try
            {
                var atividade = db.Atividades.Find(id);
                return View(atividade);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao abrir atividade.";
                return RedirectToAction("MinhasFuncionario");
            }
        }

        [HttpPost]
        public ActionResult Finalizar(Atividade atividade)
        {
            try
            {
                var atividadeDb = db.Atividades.Find(atividade.Id);

                atividadeDb.Resultado = atividade.Resultado;
                atividadeDb.Status = "Concluida";
                atividadeDb.ClienteVisualizou = false;

                db.SaveChanges();

                TempData["Sucesso"] = "Atividade finalizada!";
                return RedirectToAction("MinhasFuncionario");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao finalizar atividade.";
                return View(atividade);
            }
        }

        public ActionResult ConfirmarLeitura(int id)
        {
            try
            {
                var usuarioId = (int)Session["UsuarioId"];
                var atividade = db.Atividades.Find(id);

                if (atividade == null || atividade.ClienteId != usuarioId)
                    return RedirectToAction("Minhas");

                atividade.ClienteVisualizou = true;
                db.SaveChanges();

                return RedirectToAction("Minhas");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao confirmar leitura.";
                return RedirectToAction("Minhas");
            }
        }

        public ActionResult Arquivadas()
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Funcionario")
                    return RedirectToAction("Login", "Usuario");

                var funcionarioId = (int)Session["UsuarioId"];

                var atividades = db.Atividades
                    .Where(a => a.FuncionarioId == funcionarioId && a.ClienteVisualizou == true)
                    .ToList();

                return View(atividades);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao carregar arquivadas.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult EditarResultado(int id)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Funcionario")
                    return RedirectToAction("Login", "Usuario");

                var atividade = db.Atividades.Find(id);
                var funcionarioId = (int)Session["UsuarioId"];

                if (atividade == null || atividade.FuncionarioId != funcionarioId || atividade.ClienteVisualizou)
                    return RedirectToAction("MinhasFuncionario");

                return View(atividade);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao abrir edição.";
                return RedirectToAction("MinhasFuncionario");
            }
        }

        [HttpPost]
        public ActionResult EditarResultado(Atividade atividade)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Funcionario")
                    return RedirectToAction("Login", "Usuario");

                var atividadeDb = db.Atividades.Find(atividade.Id);
                var funcionarioId = (int)Session["UsuarioId"];

                if (atividadeDb == null || atividadeDb.FuncionarioId != funcionarioId || atividadeDb.ClienteVisualizou)
                    return RedirectToAction("MinhasFuncionario");

                atividadeDb.Resultado = atividade.Resultado;

                db.SaveChanges();

                TempData["Sucesso"] = "Resultado atualizado!";
                return RedirectToAction("MinhasFuncionario");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao editar resultado.";
                return View(atividade);
            }
        }
    }
}