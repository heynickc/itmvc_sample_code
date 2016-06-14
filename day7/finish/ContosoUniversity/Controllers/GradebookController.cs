using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
            var enrollment = _db.Enrollments.Find(id);
            return View(enrollment);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var enrollmentToUpdate = _db.Enrollments.Find(id);

            if (TryUpdateModel(enrollmentToUpdate, "", new string[] { "Grade" })) {
                try {
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */) {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(enrollmentToUpdate);
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