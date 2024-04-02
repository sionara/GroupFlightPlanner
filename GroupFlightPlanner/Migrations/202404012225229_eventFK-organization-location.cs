namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eventFKorganizationlocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "OrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "LocationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Events", "OrganizationId");
            CreateIndex("dbo.Events", "LocationId");
            AddForeignKey("dbo.Events", "LocationId", "dbo.Locations", "LocationId", cascadeDelete: true);
            AddForeignKey("dbo.Events", "OrganizationId", "dbo.Organizations", "OrganizationId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Events", "LocationId", "dbo.Locations");
            DropIndex("dbo.Events", new[] { "LocationId" });
            DropIndex("dbo.Events", new[] { "OrganizationId" });
            DropColumn("dbo.Events", "LocationId");
            DropColumn("dbo.Events", "OrganizationId");
        }
    }
}
