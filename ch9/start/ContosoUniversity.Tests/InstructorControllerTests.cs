using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ContosoUniversity.Controllers;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ToJSON;
using Xunit;
using Xunit.Abstractions;

namespace ContosoUniversity.Tests {
    public class InstructorControllerTests {
        private readonly ITestOutputHelper _output;
        private readonly ISchoolContext _fakeDb;

        public InstructorControllerTests(ITestOutputHelper output) {
            _output = output;
            _fakeDb = new FakeSchoolContext();
        }

        [Fact(Skip = "Exploratory test")]
        public void InstructorController_GetInstructorIndexData() {
            //Instructor: Kim Abercrombie Id: 9
            var controller = new InstructorController(new SchoolContext());
            var instructorIndexData = controller.GetInstructorIndexData(9, null);
            _output.WriteLine(instructorIndexData.Courses.ToJSON());
        }

        [Fact]
        public void When_selecting_instructor_instructor_courses_displayed() {
            Setup1Instructor1Course1Enrollment();

            var controller = new InstructorController(_fakeDb);
            // Testing the Controller.GetInstructorIndexData() method
            var viewModel = controller.GetInstructorIndexData(1, null);

            Assert.Equal(1, viewModel.Instructors.FirstOrDefault().Courses.Count());
        }

        [Fact]
        public void When_selecting_course_enrollments_displayed() {
            Setup1Instructor1Course1Enrollment();

            var controller = new InstructorController(_fakeDb);
            // Testing the Controller.GetInstructorIndexData() method
            var viewModel = controller.GetInstructorIndexData(1, 1);

            Assert.NotNull(viewModel.Enrollments.FirstOrDefault());
            Assert.Equal(1, viewModel.Enrollments.Count());
        }

        [Fact]
        public void When_editing_an_instructor() {
            Setup1Instructor1Course1Enrollment();
            var controller = new InstructorController(_fakeDb);
            controller.ControllerContext = new ControllerContext();
            var form = new FormCollection() {
			                {"ID", "9"},
			                {"LastName", "Abercrombie"},
			                {"FirstMidName", "Kim"},
			                {"HireDate", "3/11/1995"},
			                {"OfficeAssignment.Location", ""}
			            };
            controller.ValueProvider = form.ToValueProvider();

            var instructorId = 1;
            var selectedCourses = new string[] { "1" };

            controller.Edit(instructorId, selectedCourses);

            _output.WriteLine(controller.ModelState.ToJSON());
        }

        // Helpers
        public void Setup1Instructor1Course1Enrollment() {
            var instructor = new Instructor() {
                ID = 1,
                LastName = "Abercrombie",
                FirstMidName = "Kim",
                HireDate = new DateTime(1995, 3, 11),
                OfficeAssignment = new OfficeAssignment() {
                    Location = ""
                },
                Courses = new List<Course>() {
                    new Course() {
                        CourseID = 1,
                        Enrollments = new List<Enrollment>() {                            
                            new Enrollment() {
                                EnrollmentID = 1
                            }
                        }
                    }
                }
            };
            _fakeDb.Instructors.Add(instructor);
        }
    }
}
