namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class locationsflights : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FlightLocations",
                c => new
                    {
                        Flight_FlightId = c.Int(nullable: false),
                        Location_LocationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Flight_FlightId, t.Location_LocationId })
                .ForeignKey("dbo.Flights", t => t.Flight_FlightId, cascadeDelete: true)
                .ForeignKey("dbo.Locations", t => t.Location_LocationId, cascadeDelete: true)
                .Index(t => t.Flight_FlightId)
                .Index(t => t.Location_LocationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FlightLocations", "Location_LocationId", "dbo.Locations");
            DropForeignKey("dbo.FlightLocations", "Flight_FlightId", "dbo.Flights");
            DropIndex("dbo.FlightLocations", new[] { "Location_LocationId" });
            DropIndex("dbo.FlightLocations", new[] { "Flight_FlightId" });
            DropTable("dbo.FlightLocations");
        }
    }
}
