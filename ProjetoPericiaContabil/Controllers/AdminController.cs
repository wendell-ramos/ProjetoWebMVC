using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProjetoPericiaContabil.Models;

namespace ProjetoPericiaContabil.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Historico(string nomeUsuario, string acao, string tipoUsuario, DateTime? dataInicial, DateTime? dataFinal)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Admin")
                    return RedirectToAction("Login", "Usuario");

                var historicos = db.HistoricosAtividade.AsQueryable();

                if (!string.IsNullOrWhiteSpace(nomeUsuario))
                    historicos = historicos.Where(h => h.NomeUsuario.Contains(nomeUsuario));

                if (!string.IsNullOrWhiteSpace(acao))
                    historicos = historicos.Where(h => h.Acao.Contains(acao));

                if (!string.IsNullOrWhiteSpace(tipoUsuario))
                    historicos = historicos.Where(h => h.TipoUsuario == tipoUsuario);

                if (dataInicial.HasValue)
                    historicos = historicos.Where(h => h.DataHora >= dataInicial.Value);

                if (dataFinal.HasValue)
                {
                    var dataFinalInclusiva = dataFinal.Value.Date.AddDays(1);
                    historicos = historicos.Where(h => h.DataHora < dataFinalInclusiva);
                }

                var lista = historicos
                    .OrderByDescending(h => h.DataHora)
                    .Take(100)
                    .ToList();

                ViewBag.NomeUsuario = nomeUsuario;
                ViewBag.Acao = acao;
                ViewBag.TipoUsuario = tipoUsuario;
                ViewBag.DataInicial = dataInicial.HasValue ? dataInicial.Value.ToString("yyyy-MM-dd") : "";
                ViewBag.DataFinal = dataFinal.HasValue ? dataFinal.Value.ToString("yyyy-MM-dd") : "";
                ViewBag.AtividadesLookup = CarregarAtividadesLookup(lista);

                return View(lista);
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao carregar hist\u00f3rico do sistema.";
                return RedirectToAction("Index", "Usuario");
            }
        }

        private Dictionary<int, string> CarregarAtividadesLookup(List<HistoricoAtividade> historicos)
        {
            var ids = historicos
                .Where(h => h.AtividadeId.HasValue)
                .Select(h => h.AtividadeId.Value)
                .Distinct()
                .ToList();

            if (!ids.Any())
                return new Dictionary<int, string>();

            return db.Atividades
                .Where(a => ids.Contains(a.Id))
                .ToDictionary(a => a.Id, a => a.Titulo);
        }
    }
}
