namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class flightvalidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Flights", "FlightNumber", c => c.String(nullable: false));
            AlterColumn("dbo.Flights", "From", c => c.String(nullable: false));
            AlterColumn("dbo.Flights", "To", c => c.String(nullable: false));
            AlterColumn("dbo.Flights", "DepartureAirport", c => c.String(nullable: false));
            AlterColumn("dbo.Flights", "DestinationAirport", c => c.String(nullable: false));
            AlterColumn("dbo.Flights", "TimeZoneFrom", c => c.String(nullable: false));
            AlterColumn("dbo.Flights", "TimeZoneTo", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Flights", "TimeZoneTo", c => c.String());
            AlterColumn("dbo.Flights", "TimeZoneFrom", c => c.String());
            AlterColumn("dbo.Flights", "DestinationAirport", c => c.String());
            AlterColumn("dbo.Flights", "DepartureAirport", c => c.String());
            AlterColumn("dbo.Flights", "To", c => c.String());
            AlterColumn("dbo.Flights", "From", c => c.String());
            AlterColumn("dbo.Flights", "FlightNumber", c => c.String());
        }
    }
}
