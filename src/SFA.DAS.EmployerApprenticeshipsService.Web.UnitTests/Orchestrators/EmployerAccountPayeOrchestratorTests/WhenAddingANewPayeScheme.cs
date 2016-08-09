using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenAddingANewPayeScheme
    {
        private EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<ICookieService> _cookieService;
        private const long ExpectedAccountId = 73636363;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _cookieService = new Mock<ICookieService>();

            _employerAccountPayeOrchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object,_logger.Object, _cookieService.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledWithTheGetAccountLegalEntitesQuery()
        {
            //Arrange
            var expectedUserId = Guid.NewGuid().ToString();

            //Act
            await _employerAccountPayeOrchestrator.GetLegalEntities(ExpectedAccountId, expectedUserId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(c=>c.Id.Equals(ExpectedAccountId) && c.UserId.Equals(expectedUserId))));
        }
        
    }
}
