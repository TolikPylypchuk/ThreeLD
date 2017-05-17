using System.Data.Entity.Migrations;

namespace ThreeLD.DB.Migrations
{
    public partial class CreatedByAndProposedByFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "CreatedBy", c => c.String());
            AddColumn("dbo.Events", "ProposedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "ProposedBy");
            DropColumn("dbo.Events", "CreatedBy");
        }
    }
}
