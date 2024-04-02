namespace GroupFlightPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupsactivities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupActivities",
                c => new
                    {
                        Group_GroupId = c.Int(nullable: false),
                        Activity_ActivityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Group_GroupId, t.Activity_ActivityId })
                .ForeignKey("dbo.Groups", t => t.Group_GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Activities", t => t.Activity_ActivityId, cascadeDelete: true)
                .Index(t => t.Group_GroupId)
                .Index(t => t.Activity_ActivityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupActivities", "Activity_ActivityId", "dbo.Activities");
            DropForeignKey("dbo.GroupActivities", "Group_GroupId", "dbo.Groups");
            DropIndex("dbo.GroupActivities", new[] { "Activity_ActivityId" });
            DropIndex("dbo.GroupActivities", new[] { "Group_GroupId" });
            DropTable("dbo.GroupActivities");
        }
    }
}
