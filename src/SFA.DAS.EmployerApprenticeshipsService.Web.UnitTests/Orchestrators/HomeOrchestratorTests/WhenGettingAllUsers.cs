using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUsers;
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
        
    }
}
