namespace BouleDeNeigeService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutLancers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Lancers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTimeOffset(nullable: false, precision: 7),
                        LanceurId = c.String(maxLength: 128),
                        CibleId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Joueurs", t => t.CibleId)
                .ForeignKey("dbo.Joueurs", t => t.LanceurId)
                .Index(t => t.LanceurId)
                .Index(t => t.CibleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lancers", "LanceurId", "dbo.Joueurs");
            DropForeignKey("dbo.Lancers", "CibleId", "dbo.Joueurs");
            DropIndex("dbo.Lancers", new[] { "CibleId" });
            DropIndex("dbo.Lancers", new[] { "LanceurId" });
            DropTable("dbo.Lancers");
        }
    }
}
