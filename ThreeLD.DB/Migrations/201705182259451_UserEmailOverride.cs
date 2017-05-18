using System.Data.Entity.Migrations;

namespace ThreeLD.DB.Migrations
{
	public partial class UserEmailOverride : DbMigration
	{
		public override void Up()
		{
			AlterColumn("dbo.AspNetUsers", "Email", c => c.String(nullable: false, maxLength: 256));
		}
        
		public override void Down()
		{
			AlterColumn("dbo.AspNetUsers", "Email", c => c.String(maxLength: 256));
		}
	}
}
