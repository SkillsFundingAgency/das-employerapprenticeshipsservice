using System;
using System.Net;
using System.Web.Mvc;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Controllers;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.BaseControllerTests
{
    public class WhenReturningView : ControllerTestBase
    {
        private TestController _controller;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _controller = new TestController
            {
                ControllerContext = _controllerContext.Object
            };
        }

        [Test]
        public void ThenItShouldRethrowExceptionIfOrchestratorResponseContainsException()
        {
            Assert.Throws<Exception>(() => _controller.Test(), "Foobar");
        }

        internal class TestController : BaseController
        {
            public TestController() : base(null, null, null)
            {
            }

            public ActionResult Test()
            {
                return View(new OrchestratorResponse
                {
                    Status = HttpStatusCode.InternalServerError,
                    Exception = new Exception("Foobar")
                });
            }
        }
    }
}