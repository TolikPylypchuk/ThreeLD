namespace ThreeLD.DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationModelAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        From = c.String(nullable: false, maxLength: 128),
                        To = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.From)
                .ForeignKey("dbo.AspNetUsers", t => t.To)
                .Index(t => t.From)
                .Index(t => t.To);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "To", "dbo.AspNetUsers");
            DropForeignKey("dbo.Notifications", "From", "dbo.AspNetUsers");
            DropIndex("dbo.Notifications", new[] { "To" });
            DropIndex("dbo.Notifications", new[] { "From" });
            DropTable("dbo.Notifications");
        }
    }
}
