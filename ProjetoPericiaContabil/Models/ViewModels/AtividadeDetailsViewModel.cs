namespace ProjetoPericiaContabil.Models.ViewModels
{
    public class AtividadeDetailsViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public string TipoCalculo { get; set; }
        public string Resultado { get; set; }
        public bool ClienteVisualizou { get; set; }
        public string ClienteNome { get; set; }
        public string FuncionarioNome { get; set; }
        public string ArquivoClienteNome { get; set; }
        public string ArquivoClienteCaminho { get; set; }
        public string ArquivoResultadoNome { get; set; }
        public string ArquivoResultadoCaminho { get; set; }
        public bool PodeBaixarArquivoCliente { get; set; }
        public bool PodeBaixarArquivoResultado { get; set; }
        public bool PodeEditarResultado { get; set; }
        public bool PodeFinalizar { get; set; }
        public bool PodeConfirmarLeitura { get; set; }
    }
}
