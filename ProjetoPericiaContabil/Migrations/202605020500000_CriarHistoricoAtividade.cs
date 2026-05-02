namespace ProjetoPericiaContabil.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CriarHistoricoAtividade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HistoricosAtividade",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AtividadeId = c.Int(),
                        UsuarioId = c.Int(),
                        NomeUsuario = c.String(maxLength: 150),
                        TipoUsuario = c.String(maxLength: 50),
                        Acao = c.String(nullable: false, maxLength: 100),
                        Descricao = c.String(nullable: false, maxLength: 1000),
                        DataHora = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.HistoricosAtividade");
        }
    }
}
