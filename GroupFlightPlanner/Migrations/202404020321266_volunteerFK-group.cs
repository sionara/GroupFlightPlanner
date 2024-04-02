namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class volunteerFKgroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Volunteers", "GroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Volunteers", "GroupId");
            AddForeignKey("dbo.Volunteers", "GroupId", "dbo.Groups", "GroupId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Volunteers", "GroupId", "dbo.Groups");
            DropIndex("dbo.Volunteers", new[] { "GroupId" });
            DropColumn("dbo.Volunteers", "GroupId");
        }
    }
}
