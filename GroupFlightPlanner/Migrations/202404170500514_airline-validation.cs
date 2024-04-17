namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class airlinevalidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Airlines", "AirlineName", c => c.String(nullable: false));
            AlterColumn("dbo.Airlines", "Country", c => c.String(nullable: false));
            AlterColumn("dbo.Airlines", "Headquarters", c => c.String(nullable: false));
            AlterColumn("dbo.Airlines", "FounderName", c => c.String(nullable: false));
            AlterColumn("dbo.Airlines", "Website", c => c.String(nullable: false));
            AlterColumn("dbo.Airlines", "ContactNumber", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Airlines", "ContactNumber", c => c.String());
            AlterColumn("dbo.Airlines", "Website", c => c.String());
            AlterColumn("dbo.Airlines", "FounderName", c => c.String());
            AlterColumn("dbo.Airlines", "Headquarters", c => c.String());
            AlterColumn("dbo.Airlines", "Country", c => c.String());
            AlterColumn("dbo.Airlines", "AirlineName", c => c.String());
        }
    }
}
