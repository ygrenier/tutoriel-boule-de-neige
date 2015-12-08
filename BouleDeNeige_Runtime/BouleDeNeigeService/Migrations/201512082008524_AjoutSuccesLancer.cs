namespace BouleDeNeigeService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutSuccesLancer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lancers", "Success", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lancers", "Success");
        }
    }
}
