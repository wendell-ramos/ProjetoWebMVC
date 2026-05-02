using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetoPericiaContabil.Helpers;
using ProjetoPericiaContabil.Models;
using ProjetoPericiaContabil.Models.ViewModels;

namespace ProjetoPericiaContabil.Controllers
{
    public class AtividadeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private static readonly string[] TiposCalculoPermitidos =
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
                if (Session["Tipo"] == null)
                    return RedirectToAction("Login", "Usuario");

                var lista = db.Atividades.ToList();
                return View(lista);
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao carregar atividades.";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Details(int id)
        {
            try
            {
                if (Session["UsuarioId"] == null)
                    return RedirectToAction("Login", "Usuario");

                var atividade = db.Atividades.Find(id);

                if (atividade == null)
                {
                    TempData["Erro"] = "Atividade n\u00e3o encontrada.";
                    return RedirecionarListaPadrao();
                }

                if (!UsuarioPodeVisualizarAtividade(atividade))
                {
                    TempData["Erro"] = "Voc\u00ea n\u00e3o tem permiss\u00e3o para visualizar esta atividade.";
                    return RedirectToAction("Index", "Home");
                }

                var cliente = db.Usuarios.Find(atividade.ClienteId);
                var funcionario = atividade.FuncionarioId.HasValue
                    ? db.Usuarios.Find(atividade.FuncionarioId.Value)
                    : null;

                var model = new AtividadeDetailsViewModel
                {
                    Id = atividade.Id,
                    Titulo = atividade.Titulo,
                    Descricao = atividade.Descricao,
                    Status = atividade.Status,
                    TipoCalculo = atividade.TipoCalculo,
                    Resultado = atividade.Resultado,
                    ClienteVisualizou = atividade.ClienteVisualizou,
                    ClienteNome = cliente != null ? cliente.Nome : "Cliente n\u00e3o encontrado",
                    FuncionarioNome = funcionario != null ? funcionario.Nome : null,
                    ArquivoClienteNome = atividade.ArquivoClienteNome,
                    ArquivoClienteCaminho = atividade.ArquivoClienteCaminho,
                    ArquivoResultadoNome = atividade.ArquivoResultadoNome,
                    ArquivoResultadoCaminho = atividade.ArquivoResultadoCaminho,
                    PodeBaixarArquivoCliente = !string.IsNullOrWhiteSpace(atividade.ArquivoClienteCaminho),
                    PodeBaixarArquivoResultado = !string.IsNullOrWhiteSpace(atividade.ArquivoResultadoCaminho),
                    PodeEditarResultado = UsuarioPodeEditarResultado(atividade),
                    PodeFinalizar = UsuarioPodeFinalizar(atividade),
                    PodeConfirmarLeitura = UsuarioPodeConfirmarLeitura(atividade)
                };

                return View(model);
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao abrir detalhes da atividade.";
                return RedirecionarListaPadrao();
            }
        }

        public ActionResult Create()
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Cliente")
                    return RedirectToAction("Login", "Usuario");

                return View();
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao abrir cria\u00e7\u00e3o de atividade.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(Atividade atividade, HttpPostedFileBase arquivoCliente)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Cliente")
                    return RedirectToAction("Login", "Usuario");

                atividade.TipoCalculo = NormalizarTipoCalculo(atividade.TipoCalculo);

                if (!TipoCalculoEhPermitido(atividade.TipoCalculo))
                {
                    ViewBag.Erro = "Tipo de c\u00e1lculo inv\u00e1lido.";
                    return View(atividade);
                }

                var usuarioId = (int)Session["UsuarioId"];

                atividade.ClienteId = usuarioId;
                atividade.Status = "EmAnalise";
                atividade.FuncionarioId = null;
                atividade.Resultado = null;
                atividade.ClienteVisualizou = false;

                if (!ModelState.IsValid)
                {
                    return View(atividade);
                }

                var arquivoSalvo = UploadHelper.SalvarArquivo(
                    arquivoCliente,
                    "~/Uploads/Atividades/",
                    UploadHelper.ExtensoesClientePermitidas);

                if (arquivoSalvo != null)
                {
                    atividade.ArquivoClienteNome = arquivoSalvo.NomeOriginal;
                    atividade.ArquivoClienteCaminho = arquivoSalvo.CaminhoVirtual;
                }

                db.Atividades.Add(atividade);
                db.SaveChanges();

                TempData["Sucesso"] = "Atividade criada com sucesso!";
                return RedirectToAction("Minhas");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao criar atividade: " + ex.Message;
                return View(atividade);
            }
        }

        public ActionResult Pegar(int id)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Funcionario")
                    return RedirectToAction("Login", "Usuario");

                var atividade = db.Atividades.Find(id);

                if (atividade == null)
                {
                    TempData["Erro"] = "Atividade n\u00e3o encontrada.";
                    return RedirectToAction("Disponiveis");
                }

                var funcionarioId = (int)Session["UsuarioId"];
                var funcionario = db.Usuarios.Find(funcionarioId);

                if (funcionario == null)
                {
                    TempData["Erro"] = "Funcion\u00e1rio n\u00e3o encontrado.";
                    return RedirectToAction("Disponiveis");
                }

                var cargoFuncionario = NormalizarTipoCalculo(funcionario.Cargo);
                var tipoAtividade = NormalizarTipoCalculo(atividade.TipoCalculo);

                if (!TipoCalculoEhPermitido(cargoFuncionario) || !TipoCalculoEhPermitido(tipoAtividade))
                {
                    TempData["Erro"] = "Cargo ou tipo de c\u00e1lculo inv\u00e1lido.";
                    return RedirectToAction("Disponiveis");
                }

                if (cargoFuncionario != tipoAtividade)
                {
                    TempData["Erro"] = "Voc\u00ea n\u00e3o pode pegar essa atividade!";
                    return RedirectToAction("Disponiveis");
                }

                funcionario.Cargo = cargoFuncionario;
                atividade.TipoCalculo = tipoAtividade;
                atividade.FuncionarioId = funcionario.Id;
                atividade.Status = "EmElaboracao";

                db.SaveChanges();

                TempData["Sucesso"] = "Atividade atribu\u00edda com sucesso!";
                return RedirectToAction("Disponiveis");
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao pegar atividade.";
                return RedirectToAction("Disponiveis");
            }
        }

        public ActionResult Minhas()
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Cliente")
                    return RedirectToAction("Login", "Usuario");

                var usuarioId = (int)Session["UsuarioId"];

                var atividades = db.Atividades
                    .Where(a => a.ClienteId == usuarioId)
                    .ToList();

                return View(atividades);
            }
            catch (Exception)
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
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao carregar atividades dispon\u00edveis.";
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
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao carregar suas atividades.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Concluir(int id)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Funcionario")
                    return RedirectToAction("Login", "Usuario");

                var atividade = db.Atividades.Find(id);

                if (atividade == null)
                    return RedirectToAction("MinhasFuncionario");

                var funcionarioId = (int)Session["UsuarioId"];

                if (atividade.FuncionarioId != funcionarioId)
                    return RedirectToAction("MinhasFuncionario");

                atividade.Status = "Concluida";
                db.SaveChanges();

                TempData["Sucesso"] = "Atividade conclu\u00edda!";
                return RedirectToAction("MinhasFuncionario");
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao concluir atividade.";
                return RedirectToAction("MinhasFuncionario");
            }
        }

        public ActionResult Finalizar(int id)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Funcionario")
                    return RedirectToAction("Login", "Usuario");

                var atividade = db.Atividades.Find(id);
                var funcionarioId = (int)Session["UsuarioId"];

                if (atividade == null || atividade.FuncionarioId != funcionarioId)
                    return RedirectToAction("MinhasFuncionario");

                return View(atividade);
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao abrir atividade.";
                return RedirectToAction("MinhasFuncionario");
            }
        }

        [HttpPost]
        public ActionResult Finalizar(Atividade atividade, HttpPostedFileBase arquivoResultado)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Funcionario")
                    return RedirectToAction("Login", "Usuario");

                var atividadeDb = db.Atividades.Find(atividade.Id);
                var funcionarioId = (int)Session["UsuarioId"];

                if (atividadeDb == null || atividadeDb.FuncionarioId != funcionarioId)
                    return RedirectToAction("MinhasFuncionario");

                if (string.IsNullOrWhiteSpace(atividade.Resultado))
                {
                    ViewBag.Erro = "Informe o resultado da atividade.";
                    return View(atividadeDb);
                }

                var arquivoSalvo = UploadHelper.SalvarArquivo(
                    arquivoResultado,
                    "~/Uploads/Resultados/",
                    UploadHelper.ExtensoesResultadoPermitidas);

                atividadeDb.Resultado = atividade.Resultado;
                atividadeDb.Status = "Concluida";
                atividadeDb.ClienteVisualizou = false;

                if (arquivoSalvo != null)
                {
                    atividadeDb.ArquivoResultadoNome = arquivoSalvo.NomeOriginal;
                    atividadeDb.ArquivoResultadoCaminho = arquivoSalvo.CaminhoVirtual;
                }

                db.SaveChanges();

                TempData["Sucesso"] = "Atividade finalizada!";
                return RedirectToAction("MinhasFuncionario");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao finalizar atividade: " + ex.Message;

                var atividadeDb = db.Atividades.Find(atividade.Id);
                return View(atividadeDb ?? atividade);
            }
        }

        public ActionResult ConfirmarLeitura(int id)
        {
            try
            {
                if (Session["Tipo"]?.ToString() != "Cliente")
                    return RedirectToAction("Login", "Usuario");

                var usuarioId = (int)Session["UsuarioId"];
                var atividade = db.Atividades.Find(id);

                if (atividade == null || atividade.ClienteId != usuarioId)
                    return RedirectToAction("Minhas");

                atividade.ClienteVisualizou = true;
                db.SaveChanges();

                return RedirectToAction("Minhas");
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao confirmar leitura.";
                return RedirectToAction("Minhas");
            }
        }

        public ActionResult BaixarArquivoCliente(int id)
        {
            return BaixarArquivo(id, "cliente");
        }

        public ActionResult BaixarArquivoResultado(int id)
        {
            return BaixarArquivo(id, "resultado");
        }

        public ActionResult DownloadArquivo(int id, string tipo)
        {
            return BaixarArquivo(id, tipo);
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
            catch (Exception)
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
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao abrir edi\u00e7\u00e3o.";
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
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao editar resultado.";
                return View(atividade);
            }
        }

        private ActionResult BaixarArquivo(int id, string tipo)
        {
            try
            {
                if (Session["UsuarioId"] == null)
                    return RedirectToAction("Login", "Usuario");

                var atividade = db.Atividades.Find(id);

                if (atividade == null)
                    return HttpNotFound();

                if (!UsuarioPodeVisualizarAtividade(atividade))
                    return RedirectToAction("Index", "Home");

                string nomeArquivo;
                string caminhoArquivo;

                if (tipo == "cliente")
                {
                    nomeArquivo = atividade.ArquivoClienteNome;
                    caminhoArquivo = atividade.ArquivoClienteCaminho;
                }
                else if (tipo == "resultado")
                {
                    nomeArquivo = atividade.ArquivoResultadoNome;
                    caminhoArquivo = atividade.ArquivoResultadoCaminho;
                }
                else
                {
                    return HttpNotFound();
                }

                if (string.IsNullOrWhiteSpace(nomeArquivo) || string.IsNullOrWhiteSpace(caminhoArquivo))
                    return HttpNotFound();

                var caminhoFisico = Server.MapPath(caminhoArquivo);

                if (!System.IO.File.Exists(caminhoFisico))
                    return HttpNotFound();

                return File(caminhoFisico, MimeMapping.GetMimeMapping(nomeArquivo), nomeArquivo);
            }
            catch (Exception)
            {
                TempData["Erro"] = "Erro ao baixar arquivo.";
                return RedirecionarListaPadrao();
            }
        }

        private bool UsuarioPodeVisualizarAtividade(Atividade atividade)
        {
            if (Session["UsuarioId"] == null)
                return false;

            var usuarioId = (int)Session["UsuarioId"];
            var tipoUsuario = Session["Tipo"]?.ToString();

            if (tipoUsuario == "Admin")
                return true;

            if (tipoUsuario == "Cliente")
                return atividade.ClienteId == usuarioId;

            if (tipoUsuario == "Funcionario")
            {
                if (atividade.FuncionarioId == usuarioId)
                    return true;

                if (atividade.FuncionarioId.HasValue || atividade.Status != "EmAnalise")
                    return false;

                var funcionario = db.Usuarios.Find(usuarioId);

                if (funcionario == null)
                    return false;

                return NormalizarTipoCalculo(funcionario.Cargo) == NormalizarTipoCalculo(atividade.TipoCalculo);
            }

            return false;
        }

        private bool UsuarioPodeEditarResultado(Atividade atividade)
        {
            if (Session["Tipo"]?.ToString() != "Funcionario" || Session["UsuarioId"] == null)
                return false;

            var funcionarioId = (int)Session["UsuarioId"];
            return atividade.FuncionarioId == funcionarioId &&
                   atividade.Status == "Concluida" &&
                   !atividade.ClienteVisualizou;
        }

        private bool UsuarioPodeFinalizar(Atividade atividade)
        {
            if (Session["Tipo"]?.ToString() != "Funcionario" || Session["UsuarioId"] == null)
                return false;

            var funcionarioId = (int)Session["UsuarioId"];
            return atividade.FuncionarioId == funcionarioId &&
                   atividade.Status == "EmElaboracao";
        }

        private bool UsuarioPodeConfirmarLeitura(Atividade atividade)
        {
            if (Session["Tipo"]?.ToString() != "Cliente" || Session["UsuarioId"] == null)
                return false;

            var clienteId = (int)Session["UsuarioId"];
            return atividade.ClienteId == clienteId &&
                   atividade.Status == "Concluida" &&
                   !atividade.ClienteVisualizou;
        }

        private ActionResult RedirecionarListaPadrao()
        {
            var tipoUsuario = Session["Tipo"]?.ToString();

            if (tipoUsuario == "Admin")
                return RedirectToAction("Index", "Usuario");

            if (tipoUsuario == "Cliente")
                return RedirectToAction("Minhas");

            if (tipoUsuario == "Funcionario")
                return RedirectToAction("Disponiveis");

            return RedirectToAction("Login", "Usuario");
        }

        private static bool TipoCalculoEhPermitido(string tipoCalculo)
        {
            return TiposCalculoPermitidos.Contains(tipoCalculo);
        }

        private static string NormalizarTipoCalculo(string tipoCalculo)
        {
            if (string.IsNullOrWhiteSpace(tipoCalculo))
                return tipoCalculo;

            tipoCalculo = tipoCalculo.Trim();

            if (tipoCalculo == "Civel")
                return "C\u00edvel";

            if (tipoCalculo == "Tributario")
                return "Tribut\u00e1rio";

            if (tipoCalculo == "Previdenciario")
                return "Previdenci\u00e1rio";

            return tipoCalculo;
        }
    }
}
