using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ContosoUniversity.Controllers;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using Moq;
using ToJSON;
using Xunit;
using Xunit.Abstractions;

namespace ContosoUniversity.Tests {

    public class GradebookControllerTests {
        private readonly ITestOutputHelper _output;
        private readonly ISchoolContext _fakeDb;

        public GradebookControllerTests(ITestOutputHelper output) {
            _output = output;
            _fakeDb = new FakeSchoolContext();
        }

        [Fact]
        public void Gradebook_edit_student_grade() {
            var enrollment = new Enrollment() {
                EnrollmentID = 1,
                Course = new Course() {
                    Title = "Calculus"
                },
                Student = new Student() {
                    ID = 1,
                    FirstMidName = "Nick J",
                    LastName = "Chamberlain"
                },
                Grade = Grade.A
            };
            _fakeDb.Enrollments.Add(enrollment);

            var enrollmentToUpdate = _fakeDb.Enrollments
                .Single(x => x.EnrollmentID == 1);

            enrollmentToUpdate.Grade = Grade.B;

            var newGrade = _fakeDb.Enrollments
                .FirstOrDefault(x => x.EnrollmentID == 1).Grade;
            Assert.Equal(Grade.B, newGrade.Value);
        }

        [Fact]
        public void ModelState_gets_retrylimitexceeded_exception() {
            var _mockDb = new Mock<ISchoolContext>();
            _mockDb.Setup(x => x.SaveChanges()).Throws<RetryLimitExceededException>();

            // Add 1 Enrollment
            var enrollment = new Enrollment() {
                EnrollmentID = 1,
                Student = new Student() {
                    ID = 1,
                    FirstMidName = "Carson",
                    LastName = "Alexander"
                },
                Grade = Grade.A
            };
            _mockDb.Setup(x => x.Enrollments.Find(It.IsAny<int>()))
                .Returns(() => enrollment);

            var controller = new GradebookController(_mockDb.Object);
            controller.ControllerContext = new ControllerContext();
            // Update Enrollment Grade to a B
            var form = new FormCollection() {
		            {"Grade", "B"},
		        };
            controller.ValueProvider = form.ToValueProvider();

            controller.EditPost(1);

            Assert.Equal(
                "Unable to save changes. Try again, and if the problem persists, see your system administrator.",
                controller.ModelState[""].Errors[0].ErrorMessage);
        }

        public void Dispose() {
            _fakeDb.Dispose();
        }

        // Helpers

        private void Setup1InstructorManyCourses() {
            var instructor = new Instructor() {
                ID = 1,
                Courses = new List<Course>() {
                    new Course() {
                        CourseID = 1,
                        Enrollments = new List<Enrollment>() {
                            new Enrollment() {
                                EnrollmentID = 1,
                                Student = new Student() {
                                    ID = 1,
                                    FirstMidName = "Carson",
                                    LastName = "Alexander"
                                },
                                Grade = Grade.A
                            },
                            new Enrollment() {
                                EnrollmentID = 2,
                                Student = new Student() {
                                    ID = 2,
                                    FirstMidName = "Meredith",
                                    LastName = "Alonso"
                                },
                                Grade = Grade.B
                            },
                            new Enrollment() {
                                EnrollmentID = 3,
                                Student = new Student() {
                                    ID = 2,
                                    FirstMidName = "Arturo",
                                    LastName = "Anand"
                                },
                                Grade = Grade.B
                            },
                            new Enrollment() {
                                EnrollmentID = 4,
                                Student = new Student() {
                                    ID = 2,
                                    FirstMidName = "Gytis",
                                    LastName = "Barzdukas"
                                },
                                Grade = Grade.C
                            }
                        }
                    },
                    new Course() {
                        CourseID = 2,
                        Enrollments = new List<Enrollment>() {
                            new Enrollment() {
                                EnrollmentID = 5,
                                Student = new Student() {
                                    ID = 1,
                                    FirstMidName = "Carson",
                                    LastName = "Alexander"
                                },
                                Grade = Grade.A
                            },
                            new Enrollment() {
                                EnrollmentID = 6,
                                Student = new Student() {
                                    ID = 2,
                                    FirstMidName = "Meredith",
                                    LastName = "Alonso"
                                },
                                Grade = Grade.B
                            },
                            new Enrollment() {
                                EnrollmentID = 7,
                                Student = new Student() {
                                    ID = 2,
                                    FirstMidName = "Arturo",
                                    LastName = "Anand"
                                },
                                Grade = Grade.B
                            },
                            new Enrollment() {
                                EnrollmentID = 8,
                                Student = new Student() {
                                    ID = 2,
                                    FirstMidName = "Gytis",
                                    LastName = "Barzdukas"
                                },
                                Grade = Grade.C
                            }
                        }
                    }
                }
            };
            _fakeDb.Instructors.Add(instructor);
        }
    }
}
