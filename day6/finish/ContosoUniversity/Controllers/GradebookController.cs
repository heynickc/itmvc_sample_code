using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;

namespace ContosoUniversity.Controllers {
    public class GradebookController : Controller {
        private readonly ISchoolContext _db;

        public GradebookController(ISchoolContext db) {
            _db = db;
        }

        // GET: Gradebook
        public ActionResult Index(int? id, int? courseId) {
            var viewModel = GetGradebookViewModel(id, courseId);
            return View(viewModel);
        }

        // GET: Gradebook/Edit/**enrollmentId**
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tempModel = new Enrollment() {
                Student = new Student() {
                    FirstMidName = "Nick J",
                    LastName = "Chamberlain"
                },
                Grade = Grade.A
            };
            return View(tempModel);
        }

        public GradebookViewModel GetGradebookViewModel(int? instructorId, int? courseId) {
            var viewModel = new GradebookViewModel();

            if (instructorId != null) {
                ViewBag.InstructorID = instructorId.Value;
                viewModel.Instructor = _db.Instructors
                    .Single(x => x.ID == instructorId);
                viewModel.Courses = viewModel.Instructor.Courses;
            }

            if (courseId != null) {
                ViewBag.CourseID = courseId.Value;
                viewModel.Enrollments = viewModel.Courses
                    .Single(x => x.CourseID == courseId)
                    .Enrollments;
            }
            return viewModel;
        }
    }
}