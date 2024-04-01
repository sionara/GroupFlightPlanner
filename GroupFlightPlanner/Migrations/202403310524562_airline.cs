namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class airline : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Airlines",
                c => new
                    {
                        AirlineId = c.Int(nullable: false, identity: true),
                        AirlineName = c.String(),
                        Country = c.String(),
                        Headquarters = c.String(),
                        FounderName = c.String(),
                        FoundingYear = c.DateTime(nullable: false),
                        Website = c.String(),
                        ContactNumber = c.String(),
                    })
                .PrimaryKey(t => t.AirlineId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Airlines");
        }
    }
}
