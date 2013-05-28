namespace EsterCommon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class start : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AclItems",
                c => new
                    {
                        AclObjectID = c.Guid(nullable: false),
                        AclSubjectID = c.Guid(nullable: false),
                        ActionID = c.Int(nullable: false),
                        Action = c.Int(nullable: false),
                        Access = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.AclObjectID, t.AclSubjectID, t.ActionID })
                .ForeignKey("dbo.AclSubjects", t => t.AclSubjectID, cascadeDelete: true)
                .ForeignKey("dbo.AclObjects", t => t.AclObjectID, cascadeDelete: true)
                .Index(t => t.AclSubjectID)
                .Index(t => t.AclObjectID);
            
            CreateTable(
                "dbo.AclSubjects",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        FullName = c.String(),
                        Department = c.String(),
                        Position = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AccessCards",
                c => new
                    {
                        SiteCode = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        AccessCardState = c.Int(nullable: false),
                        Person_Id = c.Guid(),
                    })
                .PrimaryKey(t => new { t.SiteCode, t.Number })
                .ForeignKey("dbo.AclSubjects", t => t.Person_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "dbo.IdentityCards",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CardType = c.Int(nullable: false),
                        Series = c.String(),
                        Number = c.String(),
                        IssuedBy = c.String(),
                        IssueDate = c.DateTime(),
                        ExpirationDate = c.DateTime(),
                        Photo = c.Binary(),
                        Person_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AclSubjects", t => t.Person_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "dbo.AclObjects",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Mode = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PersonPersonGroups",
                c => new
                    {
                        Person_Id = c.Guid(nullable: false),
                        PersonGroup_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.PersonGroup_Id })
                .ForeignKey("dbo.AclSubjects", t => t.Person_Id)
                .ForeignKey("dbo.AclSubjects", t => t.PersonGroup_Id)
                .Index(t => t.Person_Id)
                .Index(t => t.PersonGroup_Id);
            
            CreateTable(
                "dbo.CardReaderGroupCardReaders",
                c => new
                    {
                        CardReaderGroup_Id = c.Guid(nullable: false),
                        CardReader_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CardReaderGroup_Id, t.CardReader_Id })
                .ForeignKey("dbo.AclObjects", t => t.CardReaderGroup_Id)
                .ForeignKey("dbo.AclObjects", t => t.CardReader_Id)
                .Index(t => t.CardReaderGroup_Id)
                .Index(t => t.CardReader_Id);
            
            CreateTable(
                "dbo.IntruderAlarmAreaGroupIntruderAlarmAreas",
                c => new
                    {
                        IntruderAlarmAreaGroup_Id = c.Guid(nullable: false),
                        IntruderAlarmArea_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.IntruderAlarmAreaGroup_Id, t.IntruderAlarmArea_Id })
                .ForeignKey("dbo.AclObjects", t => t.IntruderAlarmAreaGroup_Id)
                .ForeignKey("dbo.AclObjects", t => t.IntruderAlarmArea_Id)
                .Index(t => t.IntruderAlarmAreaGroup_Id)
                .Index(t => t.IntruderAlarmArea_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IntruderAlarmAreaGroupIntruderAlarmAreas", "IntruderAlarmArea_Id", "dbo.AclObjects");
            DropForeignKey("dbo.IntruderAlarmAreaGroupIntruderAlarmAreas", "IntruderAlarmAreaGroup_Id", "dbo.AclObjects");
            DropForeignKey("dbo.CardReaderGroupCardReaders", "CardReader_Id", "dbo.AclObjects");
            DropForeignKey("dbo.CardReaderGroupCardReaders", "CardReaderGroup_Id", "dbo.AclObjects");
            DropForeignKey("dbo.AclItems", "AclObjectID", "dbo.AclObjects");
            DropForeignKey("dbo.IdentityCards", "Person_Id", "dbo.AclSubjects");
            DropForeignKey("dbo.AccessCards", "Person_Id", "dbo.AclSubjects");
            DropForeignKey("dbo.PersonPersonGroups", "PersonGroup_Id", "dbo.AclSubjects");
            DropForeignKey("dbo.PersonPersonGroups", "Person_Id", "dbo.AclSubjects");
            DropForeignKey("dbo.AclItems", "AclSubjectID", "dbo.AclSubjects");
            DropIndex("dbo.IntruderAlarmAreaGroupIntruderAlarmAreas", new[] { "IntruderAlarmArea_Id" });
            DropIndex("dbo.IntruderAlarmAreaGroupIntruderAlarmAreas", new[] { "IntruderAlarmAreaGroup_Id" });
            DropIndex("dbo.CardReaderGroupCardReaders", new[] { "CardReader_Id" });
            DropIndex("dbo.CardReaderGroupCardReaders", new[] { "CardReaderGroup_Id" });
            DropIndex("dbo.AclItems", new[] { "AclObjectID" });
            DropIndex("dbo.IdentityCards", new[] { "Person_Id" });
            DropIndex("dbo.AccessCards", new[] { "Person_Id" });
            DropIndex("dbo.PersonPersonGroups", new[] { "PersonGroup_Id" });
            DropIndex("dbo.PersonPersonGroups", new[] { "Person_Id" });
            DropIndex("dbo.AclItems", new[] { "AclSubjectID" });
            DropTable("dbo.IntruderAlarmAreaGroupIntruderAlarmAreas");
            DropTable("dbo.CardReaderGroupCardReaders");
            DropTable("dbo.PersonPersonGroups");
            DropTable("dbo.AclObjects");
            DropTable("dbo.IdentityCards");
            DropTable("dbo.AccessCards");
            DropTable("dbo.AclSubjects");
            DropTable("dbo.AclItems");
        }
    }
}
