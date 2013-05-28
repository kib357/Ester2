namespace EsterServer.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class start : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AclItems", "Schedule_Id", c => c.Int());
            CreateIndex("dbo.AclItems", "Schedule_Id");
            AddForeignKey("dbo.AclItems", "Schedule_Id", "dbo.Schedules", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AclItems", "Schedule_Id", "dbo.Schedules");
            DropIndex("dbo.AclItems", new[] { "Schedule_Id" });
            DropColumn("dbo.AclItems", "Schedule_Id");
            DropTable("dbo.Schedules");
        }
    }
}
