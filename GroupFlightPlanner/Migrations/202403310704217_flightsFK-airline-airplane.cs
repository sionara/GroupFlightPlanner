namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class flightsFKairlineairplane : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Flights", "AirlineId", c => c.Int(nullable: false));
            AddColumn("dbo.Flights", "AirplaneId", c => c.Int(nullable: false));
            CreateIndex("dbo.Flights", "AirlineId");
            CreateIndex("dbo.Flights", "AirplaneId");
            AddForeignKey("dbo.Flights", "AirlineId", "dbo.Airlines", "AirlineId", cascadeDelete: true);
            AddForeignKey("dbo.Flights", "AirplaneId", "dbo.Airplanes", "AirplaneId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Flights", "AirplaneId", "dbo.Airplanes");
            DropForeignKey("dbo.Flights", "AirlineId", "dbo.Airlines");
            DropIndex("dbo.Flights", new[] { "AirplaneId" });
            DropIndex("dbo.Flights", new[] { "AirlineId" });
            DropColumn("dbo.Flights", "AirplaneId");
            DropColumn("dbo.Flights", "AirlineId");
        }
    }
}
