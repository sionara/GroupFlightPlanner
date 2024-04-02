namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class organization : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        OrganizationId = c.Int(nullable: false, identity: true),
                        OrganizationName = c.String(),
                        OrganizationContact = c.String(),
                    })
                .PrimaryKey(t => t.OrganizationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Organizations");
        }
    }
}
