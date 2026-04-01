using ProjetoPericiaContabil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPericiaContabil.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            if (Session["Tipo"]?.ToString() != "Admin")
                return RedirectToAction("Login");

            var usuarios = db.Usuarios.ToList();
            return View(usuarios);
        }
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Usuario usuario)
        {
            usuario.Tipo = "Cliente"; // TODO mundo começa cliente

            db.Usuarios.Add(usuario);
            db.SaveChanges();

            return RedirectToAction("Login");
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string senha)
        {
            var user = db.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);

            if (user != null)
            {
                Session["UsuarioId"] = user.Id;
                Session["Tipo"] = user.Tipo;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Login inválido";
            return View();
        }
        public ActionResult TornarFuncionario(int id)
        {
            var user = db.Usuarios.Find(id);

            user.Tipo = "Funcionario";
            user.Cargo = "Civel"; // 👈 GARANTE ISSO

            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}