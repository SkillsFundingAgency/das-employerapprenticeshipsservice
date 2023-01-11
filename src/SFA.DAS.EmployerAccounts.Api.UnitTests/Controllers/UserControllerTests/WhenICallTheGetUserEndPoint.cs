using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

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

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetUserByEmailQuery>())).ReturnsAsync(_response);

            _controller = new UserController(_mediator.Object);
        }

        [Test]
        public async Task ThenShouldReturnAUser()
        {
            var result = await _controller.Get("Email@Test.com") as OkNegotiatedContentResult<User>; 

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.SameAs(_user));
        }
    }
}