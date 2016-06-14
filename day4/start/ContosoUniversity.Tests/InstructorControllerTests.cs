using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ContosoUniversity.Controllers;
using ContosoUniversity.DAL;
using ToJSON;
using Xunit;
using Xunit.Abstractions;

namespace ContosoUniversity.Tests {
    public class InstructorControllerTests {
        private readonly ITestOutputHelper _output;

        public InstructorControllerTests(ITestOutputHelper output) {
            _output = output;
        }

        [Fact]
        public void InstructorController_GetInstructorIndexData() {
            //Instructor: Kim Abercrombie Id: 9
            var controller = new InstructorController(new SchoolContext());
            var instructorIndexData = controller.GetInstructorIndexData(9, null);
            _output.WriteLine(instructorIndexData.Courses.ToJSON());
        }

        [Fact]
        public void InstructorController_Index_returns_correct_viewBag_values() {
            var controller = new InstructorController(new SchoolContext());
            var result = (ViewResult)controller.Index(9, null);
            Assert.Equal(9, result.ViewBag.InstructorId);
        }
    }
}
