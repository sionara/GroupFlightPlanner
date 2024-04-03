namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eventsgroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventGroups",
                c => new
                    {
                        Event_EventId = c.Int(nullable: false),
                        Group_GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Event_EventId, t.Group_GroupId })
                .ForeignKey("dbo.Events", t => t.Event_EventId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.Group_GroupId, cascadeDelete: true)
                .Index(t => t.Event_EventId)
                .Index(t => t.Group_GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventGroups", "Group_GroupId", "dbo.Groups");
            DropForeignKey("dbo.EventGroups", "Event_EventId", "dbo.Events");
            DropIndex("dbo.EventGroups", new[] { "Group_GroupId" });
            DropIndex("dbo.EventGroups", new[] { "Event_EventId" });
            DropTable("dbo.EventGroups");
        }
    }
}
