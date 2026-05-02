namespace ProjetoPericiaContabil.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AdicionarArquivosAtividade : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Atividades", "ArquivoClienteNome", c => c.String(maxLength: 255));
            AddColumn("dbo.Atividades", "ArquivoClienteCaminho", c => c.String(maxLength: 500));
            AddColumn("dbo.Atividades", "ArquivoResultadoNome", c => c.String(maxLength: 255));
            AddColumn("dbo.Atividades", "ArquivoResultadoCaminho", c => c.String(maxLength: 500));
        }

        public override void Down()
        {
            DropColumn("dbo.Atividades", "ArquivoResultadoCaminho");
            DropColumn("dbo.Atividades", "ArquivoResultadoNome");
            DropColumn("dbo.Atividades", "ArquivoClienteCaminho");
            DropColumn("dbo.Atividades", "ArquivoClienteNome");
        }
    }
}
