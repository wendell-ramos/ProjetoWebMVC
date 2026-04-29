using ProjetoPericiaContabil.Helpers;
using ProjetoPericiaContabil.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ProjetoPericiaContabil.Controllers
{
    public class UsuarioController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Admin")
                    return RedirectToAction("Login");

                var usuarios = db.Usuarios.ToList();
                return View(usuarios);
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao carregar usuários.";
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
                    ViewBag.Erro = "Este e-mail já está cadastrado.";
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
                ViewBag.Erro = "Erro ao cadastrar usuário: " + ex.Message;

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

        public ActionResult TornarFuncionario(int id)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Admin")
                    return RedirectToAction("Login");

                var user = db.Usuarios.Find(id);

                if (user == null)
                {
                    TempData["Erro"] = "Usuário não encontrado.";
                    return RedirectToAction("Index");
                }

                user.Tipo = "Funcionario";
                user.Cargo = "Civel";

                db.SaveChanges();

                TempData["Sucesso"] = "Usuário atualizado para funcionário.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao alterar usuário.";
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
    }
}