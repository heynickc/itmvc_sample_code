using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ContosoUniversity.Controllers;
using ToJSON;
using Xunit;
using Xunit.Abstractions;

namespace ContosoUniversity.Tests {
    public class HomeControllerTests {
        private readonly ITestOutputHelper _output;
        public HomeControllerTests(ITestOutputHelper output) {
            _output = output;
        }

        [Fact(Skip = "Exploratory test")]
        public void Write_a_first_test_that_passes() {
            var controller = new HomeController();
            var index = (ViewResult)controller.Index();
            _output.WriteLine(index.ToJSON());
        }
    }
}