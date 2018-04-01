namespace EmotionPlatzi.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambiosEnEmoFaces : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Homes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WelcomeMessage = c.String(),
                        FooterMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Homes");
        }
    }
}
