namespace ThreeLD.DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
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
