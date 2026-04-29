namespace ProjetoPericiaContabil.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdicionandoValidacoesESenhaHash : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Atividades", "Titulo", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Atividades", "Descricao", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.Atividades", "Status", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Atividades", "TipoCalculo", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Atividades", "Resultado", c => c.String(maxLength: 3000));
            AlterColumn("dbo.Usuarios", "Nome", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Usuarios", "Email", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Usuarios", "Senha", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.Usuarios", "Tipo", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Usuarios", "Cargo", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Usuarios", "Cargo", c => c.String());
            AlterColumn("dbo.Usuarios", "Tipo", c => c.String());
            AlterColumn("dbo.Usuarios", "Senha", c => c.String());
            AlterColumn("dbo.Usuarios", "Email", c => c.String());
            AlterColumn("dbo.Usuarios", "Nome", c => c.String());
            AlterColumn("dbo.Atividades", "Resultado", c => c.String());
            AlterColumn("dbo.Atividades", "TipoCalculo", c => c.String());
            AlterColumn("dbo.Atividades", "Status", c => c.String());
            AlterColumn("dbo.Atividades", "Descricao", c => c.String());
            AlterColumn("dbo.Atividades", "Titulo", c => c.String());
        }
    }
}
