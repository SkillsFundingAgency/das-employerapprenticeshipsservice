using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUser;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetUserTest
{
    class WhenIGetAUser
    {
        private Mock<IUserRepository> _repository;
        private GetUserRequestHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IUserRepository>();
            _handler = new GetUserRequestHandler(_repository.Object);
        }

        [Test]
        public async Task ThenIShouldGetTheUsersDetails()
        {
            //Assign
            var request = new GetUserRequest{ UserId = 2};
            var user = new User();
            _repository.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(user);

            //Act
            var result = await _handler.Handle(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user, result.User);
            _repository.Verify(x => x.GetUserById(request.UserId), Times.Once);
        }

        [Test]
        public async Task ThenIShouldGetNullIfTheUserCannotBeFound()
        {
            //Assign
            var request = new GetUserRequest { UserId = 2 };
            var user = new User();
            _repository.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(null);

            //Act
            var result = await _handler.Handle(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.User);
            _repository.Verify(x => x.GetUserById(request.UserId), Times.Once);
        }
    }
}
