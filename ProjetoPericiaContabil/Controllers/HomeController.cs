using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPericiaContabil.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var tipo = Session["Tipo"]?.ToString();

            if (tipo == null)
                return RedirectToAction("Login", "Usuario");

            if (tipo == "Admin")
                return RedirectToAction("Index", "Usuario");

            if (tipo == "Funcionario")
                return RedirectToAction("Disponiveis", "Atividade");

            if (tipo == "Cliente")
            {
                return RedirectToAction("Minhas", "Atividade");
            }

            return RedirectToAction("Login", "Usuario");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}