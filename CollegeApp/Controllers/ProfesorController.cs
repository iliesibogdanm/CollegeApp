using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CollegeApp.DAL;
using CollegeApp.Models;
using CollegeApp.ViewModels;

namespace CollegeApp.Controllers
{
    public class ProfesorController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Profesor
        public ActionResult Index(int? id, int? courseID)
        {
            var viewModel = new ProfesorIndexData();
            viewModel.Profesors = db.Profesors
                .Include(i => i.Courses.Select(c => c.Department))
                .OrderBy(i => i.LastName);

            if (id != null)
            {
                ViewBag.ProfesorID = id.Value;
                viewModel.Courses = viewModel.Profesors.Where(
                    i => i.ID == id.Value).Single().Courses;
            }

            if (courseID != null)
            {
                ViewBag.CourseID = courseID.Value;
                // Lazy loading
                //viewModel.Enrollments = viewModel.Courses.Where(
                //    x => x.CourseID == courseID).Single().Enrollments;
                // Explicit loading
                var selectedCourse = viewModel.Courses.Where(x => x.CourseID == courseID).Single();
                db.Entry(selectedCourse).Collection(x => x.Enrollments).Load();
                foreach (Enrollment enrollment in selectedCourse.Enrollments)
                {
                    db.Entry(enrollment).Reference(x => x.Student).Load();
                }

                viewModel.Enrollments = selectedCourse.Enrollments;
            }

            return View(viewModel);
        }

        // GET: Profesor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesor profesor = db.Profesors.Find(id);
            if (profesor == null)
            {
                return HttpNotFound();
            }
            return View(profesor);
        }

        // GET: Profesor/Create
        public ActionResult Create()
        {
            var profesor = new Profesor();
            profesor.Courses = new List<Course>();
            PopulateAssignedCourseData(profesor);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,HireDate")] Profesor profesor, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                profesor.Courses = new List<Course>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = db.Courses.Find(int.Parse(course));
                    profesor.Courses.Add(courseToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                db.Profesors.Add(profesor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateAssignedCourseData(profesor);
            return View(profesor);
        }

        // GET: Profesor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesor profesor = db.Profesors
                .Include(i => i.Courses)
                .Where(i => i.ID == id)
                .Single();
            PopulateAssignedCourseData(profesor);
            if (profesor == null)
            {
                return HttpNotFound();
            }
            return View(profesor);
        }

        private void PopulateAssignedCourseData(Profesor profesor)
        {
            var allCourses = db.Courses;
            var profesorCourses = new HashSet<int>(profesor.Courses.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = profesorCourses.Contains(course.CourseID)
                });
            }
            ViewBag.Courses = viewModel;
        }

        // POST: Profesor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var profesorToUpdate = db.Profesors
               .Include(i => i.Courses)
               .Where(i => i.ID == id)
               .Single();

            if (TryUpdateModel(profesorToUpdate, "",
               new string[] { "LastName", "FirstMidName", "HireDate" }))
            {
                try
                {
                    UpdateInstructorCourses(selectedCourses, profesorToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedCourseData(profesorToUpdate);
            return View(profesorToUpdate);
        }
        private void UpdateInstructorCourses(string[] selectedCourses, Profesor profesorToUpdate)
        {
            if (selectedCourses == null)
            {
                profesorToUpdate.Courses = new List<Course>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var profesorCourses = new HashSet<int>
                (profesorToUpdate.Courses.Select(c => c.CourseID));
            foreach (var course in db.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!profesorCourses.Contains(course.CourseID))
                    {
                        profesorToUpdate.Courses.Add(course);
                    }
                }
                else
                {
                    if (profesorCourses.Contains(course.CourseID))
                    {
                        profesorToUpdate.Courses.Remove(course);
                    }
                }
            }
        }

        // GET: Profesor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesor profesor = db.Profesors.Find(id);
            if (profesor == null)
            {
                return HttpNotFound();
            }
            return View(profesor);
        }

        // POST: Profesor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profesor profesor = db.Profesors
                .Where(i => i.ID == id)
                .Single();

            db.Profesors.Remove(profesor);

            var department = db.Departments
                .Where(d => d.ProfesorID == id)
                .SingleOrDefault();
            if (department != null)
            {
                department.ProfesorID = null;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
