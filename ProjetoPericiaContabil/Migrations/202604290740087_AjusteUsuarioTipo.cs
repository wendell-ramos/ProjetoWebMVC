namespace ProjetoPericiaContabil.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjusteUsuarioTipo : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Usuarios", "Tipo", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Usuarios", "Tipo", c => c.String(nullable: false, maxLength: 20));
        }
    }
}
