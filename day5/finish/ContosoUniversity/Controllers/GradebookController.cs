using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.ViewModels;

namespace ContosoUniversity.Controllers {
    public class GradebookController : Controller {
        private readonly ISchoolContext _db;

        public GradebookController(ISchoolContext db) {
            _db = db;
        }

        // GET: Gradebook
        public ActionResult Index(int? instructorId) {
            var viewModel = new GradebookViewModel();
            if (instructorId != null) {
                viewModel.Instructor = _db.Instructors
                    .Single(x => x.ID == instructorId);
                viewModel.Courses = viewModel.Instructor.Courses;
            }

            return View(viewModel);
        }
    }
}