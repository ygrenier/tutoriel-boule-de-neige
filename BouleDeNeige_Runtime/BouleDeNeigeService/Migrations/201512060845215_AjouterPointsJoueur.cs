namespace BouleDeNeigeService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjouterPointsJoueur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Joueurs", "Points", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Joueurs", "Points");
        }
    }
}
