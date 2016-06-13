using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ToJSON;
using Xunit;
using Xunit.Abstractions;

namespace ContosoUniversity.Tests {
    public class GradebookModelTests {
        private readonly ITestOutputHelper _output;

        public GradebookModelTests(ITestOutputHelper output) {
            _output = output;
        }

        [Fact]
        public void Get_all_courses() {
            var db = new SchoolContext();
            var courses = db.Courses.ToList();
            _output.WriteLine(courses.ToJSON());
        }

        [Fact]
        public void Get_one_course() {
            var db = new SchoolContext();
            var course = db.Courses.FirstOrDefault();
            _output.WriteLine(course.ToJSON());
        }

        [Fact]
        public void Get_one_course_without_full_department_data() {
            var db = new SchoolContext();
            var course = db.Courses.FirstOrDefault();
            var courseLite = new Course() {
                CourseID = course.CourseID,
                Title = course.Title,
                Credits = course.Credits,
                DepartmentID = course.DepartmentID,
                Department = null,
                Enrollments = course.Enrollments,
                Instructors = course.Instructors
            };
            _output.WriteLine(courseLite.ToJSON());
        }

        [Fact]
        public void Get_one_course_without_full_department_and_enrollment_data() {
            var db = new SchoolContext();
            var course = db.Courses.FirstOrDefault();
            var courseLite = new Course() {
                CourseID = course.CourseID,
                Title = course.Title,
                Credits = course.Credits,
                DepartmentID = course.DepartmentID,
                Department = null,
                Enrollments = null,
                Instructors = course.Instructors
            };
            _output.WriteLine(courseLite.ToJSON());
        }

        [Fact]
        public void Drill_down_get_one_instructor_and_courses() {
            var db = new SchoolContext();
            var instructor = db.Instructors.FirstOrDefault(x => x.LastName == "Abercrombie");
            var instructorCourses = instructor.Courses.Select(x => new { x.CourseID, x.Title });
            _output.WriteLine(instructorCourses.ToJSON());
        }
    }
}
