using ProjetoPericiaContabil.Helpers;
using ProjetoPericiaContabil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjetoPericiaContabil.Controllers
{
    public class UsuarioController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private static readonly string[] CargosPermitidos =
        {
            "C\u00edvel",
            "Trabalhista",
            "Tribut\u00e1rio",
            "Previdenci\u00e1rio"
        };

        public ActionResult Index()
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Admin")
                    return RedirectToAction("Login");

                ViewBag.CargosPermitidos = CargosPermitidos;

                var usuarios = db.Usuarios.ToList();
                var atividades = db.Atividades.ToList();

                CarregarDashboardAdmin(usuarios, atividades);

                return View(usuarios);
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao carregar usu\u00e1rios.";
                return RedirectToAction("Login");
            }
        }

        public ActionResult Register()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao abrir tela de cadastro.";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult Register(Usuario usuario)
        {
            try
            {
                usuario.Tipo = "Cliente";
                usuario.Cargo = null;

                ModelState.Remove("Tipo");
                ModelState.Remove("Cargo");

                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                var emailJaExiste = db.Usuarios.Any(u => u.Email == usuario.Email);

                if (emailJaExiste)
                {
                    ViewBag.Erro = "Este e-mail j\u00e1 est\u00e1 cadastrado.";
                    return View(usuario);
                }

                usuario.Senha = CriptoHelper.HashSHA256(usuario.Senha);

                db.Usuarios.Add(usuario);
                db.SaveChanges();

                TempData["Sucesso"] = "Cadastro realizado com sucesso!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao cadastrar usu\u00e1rio: " + ex.Message;

                if (ex.InnerException != null)
                {
                    ViewBag.Erro += " | Detalhe: " + ex.InnerException.Message;
                }

                return View(usuario);
            }
        }

        public ActionResult Login()
        {
            try
            {
                if (Session["UsuarioId"] != null)
                    return RedirectToAction("Index", "Home");

                return View();
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao abrir tela de login.";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(string email, string senha)
        {
            try
            {
                string senhaCriptografada = CriptoHelper.HashSHA256(senha);

                var user = db.Usuarios.FirstOrDefault(u =>
                    u.Email == email &&
                    u.Senha == senhaCriptografada);

                if (user != null)
                {
                    Session["UsuarioId"] = user.Id;
                    Session["Tipo"] = user.Tipo;
                    Session["UsuarioLogado"] = user.Nome;

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Erro = "E-mail ou senha incorretos.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao realizar login: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult TornarFuncionario(int id, string cargo)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Admin")
                    return RedirectToAction("Login");

                cargo = NormalizarCargo(cargo);

                if (!CargoEhPermitido(cargo))
                {
                    TempData["Erro"] = "Cargo inv\u00e1lido.";
                    return RedirectToAction("Index");
                }

                var user = db.Usuarios.Find(id);

                if (user == null)
                {
                    TempData["Erro"] = "Usu\u00e1rio n\u00e3o encontrado.";
                    return RedirectToAction("Index");
                }

                user.Tipo = "Funcionario";
                user.Cargo = cargo;

                db.SaveChanges();

                TempData["Sucesso"] = "Usu\u00e1rio atualizado para funcion\u00e1rio.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao alterar usu\u00e1rio.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Logout()
        {
            try
            {
                Session.Clear();
                Session.Abandon();

                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao sair do sistema.";
                return RedirectToAction("Login");
            }
        }

        private void CarregarDashboardAdmin(List<Usuario> usuarios, List<Atividade> atividades)
        {
            var totalArquivosAnexados = atividades.Count(a => !string.IsNullOrWhiteSpace(a.ArquivoClienteCaminho)) +
                                        atividades.Count(a => !string.IsNullOrWhiteSpace(a.ArquivoResultadoCaminho));

            ViewBag.TotalUsuarios = usuarios.Count;
            ViewBag.TotalClientes = usuarios.Count(u => u.Tipo == "Cliente");
            ViewBag.TotalFuncionarios = usuarios.Count(u => u.Tipo == "Funcionario");
            ViewBag.TotalAtividades = atividades.Count;
            ViewBag.TotalConcluidas = atividades.Count(a => a.Status == "Concluida");
            ViewBag.AguardandoClienteVisualizar = atividades.Count(a => a.Status == "Concluida" && !a.ClienteVisualizou);
            ViewBag.SemFuncionarioResponsavel = atividades.Count(a => !a.FuncionarioId.HasValue);
            ViewBag.TotalArquivosAnexados = totalArquivosAnexados;

            ViewBag.AtividadesPorStatus = atividades
                .GroupBy(a => string.IsNullOrWhiteSpace(a.Status) ? "Sem status" : a.Status)
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count());

            ViewBag.AtividadesPorTipo = atividades
                .GroupBy(a => string.IsNullOrWhiteSpace(a.TipoCalculo) ? "Sem tipo" : NormalizarCargo(a.TipoCalculo))
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count());

            ViewBag.RankingFuncionarios = atividades
                .Where(a => a.FuncionarioId.HasValue)
                .GroupBy(a => a.FuncionarioId.Value)
                .Select(g =>
                {
                    var funcionario = usuarios.FirstOrDefault(u => u.Id == g.Key);
                    var nome = funcionario != null ? funcionario.Nome : "Funcion\u00e1rio removido";
                    var cargo = funcionario != null ? NormalizarCargo(funcionario.Cargo) : "";
                    return Tuple.Create(nome, cargo, g.Count());
                })
                .OrderByDescending(item => item.Item3)
                .Take(5)
                .ToList();

            ViewBag.UltimasAtividades = atividades
                .OrderByDescending(a => a.Id)
                .Take(5)
                .ToList();

            ViewBag.AtividadesAguardandoCliente = atividades
                .Where(a => a.Status == "Concluida" && !a.ClienteVisualizou)
                .OrderByDescending(a => a.Id)
                .Take(5)
                .ToList();

            ViewBag.AtividadesSemFuncionario = atividades
                .Where(a => !a.FuncionarioId.HasValue)
                .OrderByDescending(a => a.Id)
                .Take(5)
                .ToList();
        }

        private static bool CargoEhPermitido(string cargo)
        {
            return CargosPermitidos.Contains(cargo);
        }

        private static string NormalizarCargo(string cargo)
        {
            if (string.IsNullOrWhiteSpace(cargo))
                return cargo;

            cargo = cargo.Trim();

            if (cargo == "Civel")
                return "C\u00edvel";

            if (cargo == "Tributario")
                return "Tribut\u00e1rio";

            if (cargo == "Previdenciario")
                return "Previdenci\u00e1rio";

            return cargo;
        }
    }
}
