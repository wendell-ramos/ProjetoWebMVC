using System;
using System.Web.Mvc;

namespace ProjetoPericiaContabil.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var tipo = Session["Tipo"]?.ToString();

                if (tipo == null)
                    return RedirectToAction("Login", "Usuario");

                if (tipo == "Admin")
                    return RedirectToAction("Index", "Usuario");

                if (tipo == "Funcionario")
                    return RedirectToAction("Disponiveis", "Atividade");

                if (tipo == "Cliente")
                    return RedirectToAction("Minhas", "Atividade");

                return RedirectToAction("Login", "Usuario");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao carregar a página inicial.";
                return RedirectToAction("Login", "Usuario");
            }
        }

        public ActionResult About()
        {
            try
            {
                ViewBag.Message = "Your application description page.";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao carregar a página sobre.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Contact()
        {
            try
            {
                ViewBag.Message = "Your contact page.";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao carregar a página de contato.";
                return RedirectToAction("Index");
            }
        }
    }
}