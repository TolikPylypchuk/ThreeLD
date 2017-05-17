using System.Data.Entity.Migrations;

namespace ThreeLD.DB.Migrations
{
    public partial class Localization : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Preferences", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Preferences", new[] { "UserId" });
            AlterColumn("dbo.Events", "Url", c => c.String());
            AlterColumn("dbo.Preferences", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Preferences", "UserId");
            AddForeignKey("dbo.Preferences", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Preferences", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Preferences", new[] { "UserId" });
            AlterColumn("dbo.Preferences", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Events", "Url", c => c.String(nullable: false));
            CreateIndex("dbo.Preferences", "UserId");
            AddForeignKey("dbo.Preferences", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
