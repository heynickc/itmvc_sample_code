using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;
using ToJSON;
using Xunit;
using Xunit.Abstractions;

namespace ContosoUniversity.Tests {
    public class GradebookModelTests : IDisposable {
        private readonly ITestOutputHelper _output;
        private readonly ISchoolContext _fakeDb;

        public GradebookModelTests(ITestOutputHelper output) {
            _output = output;
            _fakeDb = new FakeSchoolContext();
        }

        [Fact(Skip = "Exploratory test")]
        public void Get_all_courses() {
            var db = new SchoolContext();
            var courses = db.Courses.ToList();
            _output.WriteLine(courses.ToJSON());
        }

        [Fact(Skip = "Exploratory test")]
        public void Get_one_course() {
            var db = new SchoolContext();
            var course = db.Courses.FirstOrDefault();
            _output.WriteLine(course.ToJSON());
        }

        [Fact(Skip = "Exploratory test")]
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

        [Fact(Skip = "Exploratory test")]
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

        [Fact(Skip = "Exploratory test")]
        public void Drill_down_get_one_instructor_and_courses() {
            var db = new SchoolContext();
            var instructor = db.Instructors.FirstOrDefault(x => x.LastName == "Abercrombie");
            var instructorCourses = instructor.Courses.Select(x => new { x.CourseID, x.Title });
            _output.WriteLine(instructorCourses.ToJSON());
        }

        [Fact]
        public void All_instructor_courses_returned_from_dbcontext() {
            var context = new FakeSchoolContext();
            int? id = 1;
            var instructor = new Instructor() {
                ID = 1,
                Courses = new List<Course>() {
            new Course() {
                CourseID = 1,
                Enrollments = new List<Enrollment>() {
                    new Enrollment()
                }
            }
        }
            };
            context.Instructors.Add(instructor);

            var viewModel = new InstructorIndexData();

            viewModel.Instructors = context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(c => c.Department))
                .OrderBy(i => i.LastName);
            viewModel.Courses = viewModel.Instructors.Single(i => i.ID == id.Value).Courses;
            viewModel.Enrollments = viewModel.Courses.Single(x => x.CourseID == 1).Enrollments;

            Assert.Equal(1, viewModel.Instructors.Count());
            Assert.Equal(1, viewModel.Courses.Count());
            Assert.Equal(1, viewModel.Enrollments.Count());
        }

        [Fact]
        public void Build_anonymous_type_version_of_gradebook() {

            Setup1InstructorManyCourses();
            var instructorId = 1;

            var viewModel = _fakeDb.Instructors
                .Single(x => x.ID == instructorId)
                .Courses
                .SelectMany(y => y.Enrollments)
                .Select(z => new { z.Student.FullName, z.Grade })
                .ToList();

            Assert.Equal(8, viewModel.Count);
        }

        public class GradebookViewModel {
            public Instructor Instructor { get; set; }
            public IEnumerable<Course> Courses { get; set; }
            public IEnumerable<Enrollment> Enrollments { get; set; }
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
                                Student = new Student() {
                                    ID = 1,
                                    FirstMidName = "Carson",
                                    LastName = "Alexander"
                                },
                                Grade = Grade.A
                            },
                            new Enrollment() {
                                Student = new Student() {
                                    ID = 2,
                                    FirstMidName = "Meredith",
                                    LastName = "Alonso"
                                },
                                Grade = Grade.B
                            },
                            new Enrollment() {
                                Student = new Student() {
                                    ID = 2,
                                    FirstMidName = "Arturo",
                                    LastName = "Anand"
                                },
                                Grade = Grade.B
                            },
                            new Enrollment() {
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
                                Student = new Student() {
                                    ID = 1,
                                    FirstMidName = "Carson",
                                    LastName = "Alexander"
                                },
                                Grade = Grade.A
                            },
                            new Enrollment() {
                                Student = new Student() {
                                    ID = 2,
                                    FirstMidName = "Meredith",
                                    LastName = "Alonso"
                                },
                                Grade = Grade.B
                            },
                            new Enrollment() {
                                Student = new Student() {
                                    ID = 2,
                                    FirstMidName = "Arturo",
                                    LastName = "Anand"
                                },
                                Grade = Grade.B
                            },
                            new Enrollment() {
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

        public void Dispose() {
            _fakeDb.Dispose();
        }
    }
}
