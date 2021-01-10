using System.Collections.Generic;
using CollegeApp.Models;

namespace CollegeApp.ViewModels
{
    public class ProfesorIndexData
    {
        public IEnumerable<Profesor> Profesors { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}