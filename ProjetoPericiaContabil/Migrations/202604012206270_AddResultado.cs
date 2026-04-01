namespace ProjetoPericiaContabil.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResultado : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Atividades", "Resultado", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Atividades", "Resultado");
        }
    }
}
