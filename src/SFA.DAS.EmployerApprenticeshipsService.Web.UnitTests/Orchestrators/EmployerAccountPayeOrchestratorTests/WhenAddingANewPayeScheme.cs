using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeWithExistingLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
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
        private const string ExpectedEmpref = "123/DFDS";
        private const string ExpectedUserId = "someid";

        [SetUp]
        public void Arrange()
        {
            
            _logger = new Mock<ILogger>();
            _cookieService = new Mock<ICookieService>();

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    Entites = new LegalEntities {LegalEntityList = new List<LegalEntity>()}
                });

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

        [Test]
        public async Task ThenTheAddPayeToAccountForExistingLegalEntityCommandIsCalledWhenTheLegalEntityIdIsNotZero()
        {
            //Arrange
            var model = new ConfirmNewPayeScheme
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                AccountId = ExpectedAccountId,
                PayeScheme = ExpectedEmpref,
                LegalEntityId = 1
            };

            //Act
            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(model, ExpectedUserId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<AddPayeToAccountForExistingLegalEntityCommand>(c=>c.AccountId.Equals(ExpectedAccountId) && c.EmpRef.Equals(ExpectedEmpref) && c.ExternalUserId.Equals(ExpectedUserId) && c.LegalEntityId.Equals(1))),Times.Once);
        }
    }
}
