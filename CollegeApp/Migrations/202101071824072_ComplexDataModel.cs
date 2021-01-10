namespace CollegeApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComplexDataModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Department",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Budget = c.Decimal(nullable: false, storeType: "money"),
                        StartDate = c.DateTime(nullable: false),
                        ProfesorID = c.Int(),
                    })
                .PrimaryKey(t => t.DepartmentID)
                .ForeignKey("dbo.Profesor", t => t.ProfesorID)
                .Index(t => t.ProfesorID);
            
            CreateTable(
                "dbo.Profesor",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        HireDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.CourseProfesor",
                c => new
                    {
                        CourseID = c.Int(nullable: false),
                        ProfesorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseID, t.ProfesorID })
                .ForeignKey("dbo.Course", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("dbo.Profesor", t => t.ProfesorID, cascadeDelete: true)
                .Index(t => t.CourseID)
                .Index(t => t.ProfesorID);

            // Create  a department for course to point to.
            Sql("INSERT INTO dbo.Department (Name, Budget, StartDate) VALUES ('Temp', 0.00, GETDATE())");
            //  default value for FK points to department created above.
            AddColumn("dbo.Course", "DepartmentID", c => c.Int(nullable: false, defaultValue: 1));
            //AddColumn("dbo.Course", "DepartmentID", c => c.Int(nullable: false));

            AlterColumn("dbo.Course", "Title", c => c.String(maxLength: 50));
            CreateIndex("dbo.Course", "DepartmentID");
            AddForeignKey("dbo.Course", "DepartmentID", "dbo.Department", "DepartmentID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseProfesor", "ProfesorID", "dbo.Profesor");
            DropForeignKey("dbo.CourseProfesor", "CourseID", "dbo.Course");
            DropForeignKey("dbo.Course", "DepartmentID", "dbo.Department");
            DropForeignKey("dbo.Department", "ProfesorID", "dbo.Profesor");
            DropIndex("dbo.CourseProfesor", new[] { "ProfesorID" });
            DropIndex("dbo.CourseProfesor", new[] { "CourseID" });
            DropIndex("dbo.Department", new[] { "ProfesorID" });
            DropIndex("dbo.Course", new[] { "DepartmentID" });
            AlterColumn("dbo.Course", "Title", c => c.String());
            DropColumn("dbo.Course", "DepartmentID");
            DropTable("dbo.CourseProfesor");
            DropTable("dbo.Profesor");
            DropTable("dbo.Department");
        }
    }
}
