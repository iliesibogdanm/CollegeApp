namespace CollegeApp.Migrations
{
    using CollegeApp.Models;
    using CollegeApp.DAL;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SchoolContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SchoolContext context)
        {
            var students = new List<Student>
            {
                new Student { FirstMidName = "Bogdan",   LastName = "Iliesi",
                    EnrollmentDate = DateTime.Parse("2010-09-01") },
                new Student { FirstMidName = "Andrei", LastName = "Pop",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstMidName = "Ana",   LastName = "Vasilicu",
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Student { FirstMidName = "Alex",    LastName = "Andronescu",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstMidName = "Ioana",      LastName = "Potop",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstMidName = "Alin",    LastName = "Vladesu",
                    EnrollmentDate = DateTime.Parse("2011-09-01") },
                new Student { FirstMidName = "Vlad",    LastName = "Iriciuc",
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Student { FirstMidName = "Alexandra",     LastName = "Ionescu",
                    EnrollmentDate = DateTime.Parse("2005-09-01") }
            };

            students.ForEach(s => context.Students.AddOrUpdate(p => p.LastName, s));
            context.SaveChanges();

            var profesors = new List<Profesor>
            {
                new Profesor { FirstMidName = "Dumitru",     LastName = "Antol",
                    HireDate = DateTime.Parse("1995-03-11") },
                new Profesor { FirstMidName = "Andreea",    LastName = "Sovrate",
                    HireDate = DateTime.Parse("2002-07-06") },
                new Profesor { FirstMidName = "Mircea",   LastName = "Mavrodin",
                    HireDate = DateTime.Parse("1998-07-01") },
                new Profesor { FirstMidName = "Vasile", LastName = "Pop",
                    HireDate = DateTime.Parse("2001-01-15") },
                new Profesor { FirstMidName = "Mihaela",   LastName = "Pringu",
                    HireDate = DateTime.Parse("2004-02-12") }
            };
            profesors.ForEach(s => context.Profesors.AddOrUpdate(p => p.LastName, s));
            context.SaveChanges();

            var departments = new List<Department>
            {
                new Department { Name = "English",     Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    ProfesorID  = profesors.Single( i => i.LastName == "Sovrate").ID },
                new Department { Name = "Mathematics", Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    ProfesorID  = profesors.Single( i => i.LastName == "Pop").ID },
                new Department { Name = "Engineering", Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    ProfesorID  = profesors.Single( i => i.LastName == "Pringu").ID },
                new Department { Name = "Economics",   Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    ProfesorID  = profesors.Single( i => i.LastName == "Antol").ID }
            };
            departments.ForEach(s => context.Departments.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var courses = new List<Course>
            {
                new Course {CourseID = 1050, Title = "Chemistry",      Credits = 3,
                  DepartmentID = departments.Single( s => s.Name == "Engineering").DepartmentID,
                  Profesors = new List<Profesor>()
                },
                new Course {CourseID = 4022, Title = "Microeconomics", Credits = 3,
                  DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID,
                  Profesors = new List<Profesor>()
                },
                new Course {CourseID = 4041, Title = "Macroeconomics", Credits = 3,
                  DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID,
                  Profesors = new List<Profesor>()
                },
                new Course {CourseID = 1045, Title = "Calculus",       Credits = 4,
                  DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID,
                  Profesors = new List<Profesor>()
                },
                new Course {CourseID = 3141, Title = "Trigonometry",   Credits = 4,
                  DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID,
                  Profesors = new List<Profesor>()
                },
                new Course {CourseID = 2021, Title = "Composition",    Credits = 3,
                  DepartmentID = departments.Single( s => s.Name == "English").DepartmentID,
                  Profesors = new List<Profesor>()
                },
                new Course {CourseID = 2042, Title = "Literature",     Credits = 4,
                  DepartmentID = departments.Single( s => s.Name == "English").DepartmentID,
                  Profesors = new List<Profesor>()
                },
            };
            courses.ForEach(s => context.Courses.AddOrUpdate(p => p.CourseID, s));
            context.SaveChanges();



            AddOrUpdateProfesor(context, "Chemistry", "Pringu");
            AddOrUpdateProfesor(context, "Chemistry", "Mavrodin");
            AddOrUpdateProfesor(context, "Microeconomics", "Antol");
            AddOrUpdateProfesor(context, "Macroeconomics", "Antol");

            AddOrUpdateProfesor(context, "Calculus", "Pop");
            AddOrUpdateProfesor(context, "Trigonometry", "Sovrate");
            AddOrUpdateProfesor(context, "Composition", "Antol");
            AddOrUpdateProfesor(context, "Literature", "Mavrodin");

            context.SaveChanges();

            var enrollments = new List<Enrollment>
            {
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Iliesi").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID,
                    Grade = Grade.A
                },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Iliesi").ID,
                    CourseID = courses.Single(c => c.Title == "Microeconomics" ).CourseID,
                    Grade = Grade.C
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Iliesi").ID,
                    CourseID = courses.Single(c => c.Title == "Macroeconomics" ).CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                     StudentID = students.Single(s => s.LastName == "Andronescu").ID,
                    CourseID = courses.Single(c => c.Title == "Calculus" ).CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                     StudentID = students.Single(s => s.LastName == "Andronescu").ID,
                    CourseID = courses.Single(c => c.Title == "Trigonometry" ).CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Andronescu").ID,
                    CourseID = courses.Single(c => c.Title == "Composition" ).CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Pop").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Pop").ID,
                    CourseID = courses.Single(c => c.Title == "Microeconomics").CourseID,
                    Grade = Grade.B
                 },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Iriciuc").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry").CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Vladesu").ID,
                    CourseID = courses.Single(c => c.Title == "Composition").CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Potop").ID,
                    CourseID = courses.Single(c => c.Title == "Literature").CourseID,
                    Grade = Grade.B
                 }
            };

            foreach (Enrollment e in enrollments)
            {
                var enrollmentInDataBase = context.Enrollments.Where(
                    s =>
                         s.Student.ID == e.StudentID &&
                         s.Course.CourseID == e.CourseID).SingleOrDefault();
                if (enrollmentInDataBase == null)
                {
                    context.Enrollments.Add(e);
                }
            }
            context.SaveChanges();
        }

        void AddOrUpdateProfesor(SchoolContext context, string courseTitle, string profesorName)
        {
            var crs = context.Courses.SingleOrDefault(c => c.Title == courseTitle);
            var inst = crs.Profesors.SingleOrDefault(i => i.LastName == profesorName);
            if (inst == null)
                crs.Profesors.Add(context.Profesors.Single(i => i.LastName == profesorName));
        }
    }
}