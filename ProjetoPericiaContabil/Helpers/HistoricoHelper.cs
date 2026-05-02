using System;
using ProjetoPericiaContabil.Models;

namespace ProjetoPericiaContabil.Helpers
{
    public static class HistoricoHelper
    {
        public static void Registrar(
            ApplicationDbContext db,
            int? atividadeId,
            int? usuarioId,
            string nomeUsuario,
            string tipoUsuario,
            string acao,
            string descricao)
        {
            try
            {
                if (db == null)
                    return;

                var historico = new HistoricoAtividade
                {
                    AtividadeId = atividadeId,
                    UsuarioId = usuarioId,
                    NomeUsuario = nomeUsuario,
                    TipoUsuario = tipoUsuario,
                    Acao = acao,
                    Descricao = descricao,
                    DataHora = DateTime.Now
                };

                db.HistoricosAtividade.Add(historico);
                db.SaveChanges();
            }
            catch (Exception)
            {
                // O histórico não pode impedir a ação principal do sistema.
            }
        }
    }
}
