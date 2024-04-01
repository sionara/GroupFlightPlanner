namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class airplane : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Airplanes",
                c => new
                    {
                        AirplaneId = c.Int(nullable: false, identity: true),
                        AirplaneModel = c.String(),
                        RegistrationNum = c.String(),
                        ManufacturerName = c.String(),
                        ManufactureYear = c.DateTime(nullable: false),
                        MaxPassenger = c.Int(nullable: false),
                        EngineModel = c.String(),
                        Speed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Range = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.AirplaneId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Airplanes");
        }
    }
}
