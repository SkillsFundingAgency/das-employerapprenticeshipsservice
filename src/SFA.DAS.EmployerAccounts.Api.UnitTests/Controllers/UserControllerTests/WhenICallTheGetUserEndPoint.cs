using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.UserControllerTests
{
    [TestFixture]
    public class WhenICallTheGetUserEndPoint
    {
        private UserController _controller;
        private Mock<IMediator> _mediator;
        private GetUserByEmailResponse _response;
        private User _user;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();

            _user = new User
            {
              FirstName = "John",
              LastName = "Smith"
            };

            _response = new GetUserByEmailResponse() { User = _user };

            _mediator.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(_response);

            _controller = new UserController(_mediator.Object, Mock.Of<ILogger<UserController>>());
        }

        [Test]
        public async Task ThenShouldReturnAUser()
        {
            var result = await _controller.Get("Email@Test.com") as OkObjectResult; 

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.SameAs(_user));
        }
    }
}