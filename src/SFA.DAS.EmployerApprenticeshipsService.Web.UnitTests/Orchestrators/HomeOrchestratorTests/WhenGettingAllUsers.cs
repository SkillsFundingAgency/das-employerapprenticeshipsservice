using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUsers;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.HomeOrchestratorTests
{
    public class WhenGettingAllUsers
    {
        private HomeOrchestrator _homeOrchestrator;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUsersQuery>()))
                .ReturnsAsync(new List<User>
                {
                    new User {Email = "test@local.com", FirstName = "test", LastName = "tester", UserId = "1"}
                });
            _homeOrchestrator = new HomeOrchestrator(_mediator.Object);

        }

        [Test]
        public async Task ThenTheMediatorIsCalledWithTheGetUserQuery()
        {
            //Act
            await _homeOrchestrator.GetUsers();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny<GetUsersQuery>()));
        }

        [Test]
        public async Task ThenTheSignInUserViewModelIsReturned()
        {
            //Act
            var actual = await _homeOrchestrator.GetUsers();

            //Assert
            Assert.IsAssignableFrom<SignInUserViewModel>(actual);
        }

        [Test]
        public async Task TheTheViewModelIsPopulatedFromTheMediator()
        {
            //Act
            var actual = await _homeOrchestrator.GetUsers();

            //Assert
            Assert.AreEqual(1, actual.AvailableUsers.Count);
            Assert.IsTrue(actual.AvailableUsers.Any(x=>x.UserId.Equals("1") && x.Email.Equals("test@local.com") && x.FirstName.Equals("test")&& x.LastName.Equals("tester")));
            
        }
    }
}
