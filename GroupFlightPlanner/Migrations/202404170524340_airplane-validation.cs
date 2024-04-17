namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class airplanevalidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Airplanes", "AirplaneModel", c => c.String(nullable: false));
            AlterColumn("dbo.Airplanes", "RegistrationNum", c => c.String(nullable: false));
            AlterColumn("dbo.Airplanes", "ManufacturerName", c => c.String(nullable: false));
            AlterColumn("dbo.Airplanes", "EngineModel", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Airplanes", "EngineModel", c => c.String());
            AlterColumn("dbo.Airplanes", "ManufacturerName", c => c.String());
            AlterColumn("dbo.Airplanes", "RegistrationNum", c => c.String());
            AlterColumn("dbo.Airplanes", "AirplaneModel", c => c.String());
        }
    }
}
