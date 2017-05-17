using System.Data.Entity.Migrations;

namespace ThreeLD.DB.Migrations
{
    public partial class IsApprovedFieldAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "IsApproved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "IsApproved");
        }
    }
}
