using System;
using System.IO;
using System.Linq;
using System.Web;

namespace ProjetoPericiaContabil.Helpers
{
    public static class UploadHelper
    {
        public const int TamanhoMaximoBytes = 10 * 1024 * 1024;

        public static readonly string[] ExtensoesClientePermitidas =
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg"
        };

        public static readonly string[] ExtensoesResultadoPermitidas =
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx"
        };

        public static ArquivoUpload SalvarArquivo(HttpPostedFileBase arquivo, string pastaVirtual, string[] extensoesPermitidas)
        {
            if (arquivo == null || arquivo.ContentLength == 0)
                return null;

            if (arquivo.ContentLength > TamanhoMaximoBytes)
                throw new InvalidOperationException("O arquivo deve ter no máximo 10 MB.");

            var extensao = Path.GetExtension(arquivo.FileName)?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extensao) || !extensoesPermitidas.Contains(extensao))
                throw new InvalidOperationException("Extensão de arquivo não permitida.");

            var nomeOriginal = Path.GetFileName(arquivo.FileName);
            var nomeSeguro = GerarNomeSeguro(nomeOriginal, extensao);
            var pastaFisica = HttpContext.Current.Server.MapPath(pastaVirtual);

            Directory.CreateDirectory(pastaFisica);

            var caminhoFisico = Path.Combine(pastaFisica, nomeSeguro);
            arquivo.SaveAs(caminhoFisico);

            return new ArquivoUpload
            {
                NomeOriginal = nomeOriginal,
                CaminhoVirtual = VirtualPathUtility.Combine(pastaVirtual, nomeSeguro)
            };
        }

        private static string GerarNomeSeguro(string nomeOriginal, string extensao)
        {
            var nomeSemExtensao = Path.GetFileNameWithoutExtension(nomeOriginal);

            foreach (var caractereInvalido in Path.GetInvalidFileNameChars())
            {
                nomeSemExtensao = nomeSemExtensao.Replace(caractereInvalido, '_');
            }

            if (string.IsNullOrWhiteSpace(nomeSemExtensao))
                nomeSemExtensao = "arquivo";

            if (nomeSemExtensao.Length > 80)
                nomeSemExtensao = nomeSemExtensao.Substring(0, 80);

            return $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}_{nomeSemExtensao}{extensao}";
        }
    }

    public class ArquivoUpload
    {
        public string NomeOriginal { get; set; }
        public string CaminhoVirtual { get; set; }
    }
}
