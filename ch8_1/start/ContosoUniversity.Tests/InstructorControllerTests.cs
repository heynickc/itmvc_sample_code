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

        public InstructorControllerTests(ITestOutputHelper output) {
            _output = output;
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
            var fakeSchoolContext = new FakeSchoolContext();
            var instructor = new Instructor() {
                        ID = 1,
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
            fakeSchoolContext.Instructors.Add(instructor);

            var controller = new InstructorController(fakeSchoolContext);
            // Testing the Controller.GetInstructorIndexData() method
            var viewModel = controller.GetInstructorIndexData(1, null);

            Assert.Equal(1, viewModel.Instructors.FirstOrDefault().Courses.Count());
        }

        [Fact]
        public void When_selecting_course_enrollments_displayed() {
            var fakeSchoolContext = new FakeSchoolContext();
            var instructor = new Instructor() {
                        ID = 1,
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
            fakeSchoolContext.Instructors.Add(instructor);

            var controller = new InstructorController(fakeSchoolContext);
            // Testing the Controller.GetInstructorIndexData() method
            var viewModel = controller.GetInstructorIndexData(1, 1);

            Assert.NotNull(viewModel.Enrollments.FirstOrDefault());
            Assert.Equal(1, viewModel.Enrollments.Count());
        }
    }
}
