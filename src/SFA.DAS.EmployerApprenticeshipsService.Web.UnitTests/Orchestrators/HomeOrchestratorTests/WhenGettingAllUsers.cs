using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUsers;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.HomeOrchestratorTests
{
    public class WhenGettingAllUsers
    {
        private HomeOrchestrator _homeOrchestrator;
        private Mock<IMediator> _mediator;
        private User _user;
        
        [SetUp]
        public void Arrange()
        {
            _user = new User
            {
                Id = 1,
                Email = "test@local.com",
                FirstName = "test",
                LastName = "tester",
                UserRef = Guid.NewGuid().ToString()
            };
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUsersQuery>()))
                .ReturnsAsync(new GetUsersQueryResponse
                {
                    Users = new List<User>
                    {
                        _user
                    }
                });
            _homeOrchestrator = new HomeOrchestrator(_mediator.Object);

        }

        [Test]
        public async Task ThenTheMediatorIsCalledWithTheGetUserQuery()
        {
            //Act
            await _homeOrchestrator.GetUsers();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetUsersQuery>()));
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
            Assert.IsTrue(actual.AvailableUsers.Any(x => x.UserId.Equals(_user.UserRef) && x.Email.Equals(_user.Email) && x.FirstName.Equals(_user.FirstName) && x.LastName.Equals(_user.LastName)));

        }
    }
}
