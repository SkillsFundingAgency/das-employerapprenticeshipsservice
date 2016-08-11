using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeWithExistingLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenAddingAPayeScheme
    {
        private EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<ICookieService> _cookieService;
        private ConfirmNewPayeScheme _model;
        private const long ExpectedAccountId = 73636363;
        private const string ExpectedEmpref = "123/DFDS";
        private const string ExpectedUserId = "someid";

        [SetUp]
        public void Arrange()
        {
            _model = new ConfirmNewPayeScheme
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                AccountId = ExpectedAccountId,
                PayeScheme = ExpectedEmpref,
                LegalEntityId = 1,
                LegalEntityCode = "mycode",
                LegalEntityName = "name",
                LegalEntityDateOfIncorporation = new DateTime(2016,01,01),
                LegalEntityRegisteredAddress = "Test Address"
            };

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
            //Act
            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(_model, ExpectedUserId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<AddPayeToAccountForExistingLegalEntityCommand>(c=>c.AccountId.Equals(ExpectedAccountId) && c.EmpRef.Equals(ExpectedEmpref) && c.ExternalUserId.Equals(ExpectedUserId) && c.LegalEntityId.Equals(1))),Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<AddPayeToNewLegalEntityCommand>()), Times.Never);
        }

        [Test]
        public async Task ThenTheAddPayeToAccountForNewLegalEntityCommandIsCalledWhenTheLegalEntityIsZero()
        {
            //Arrange
            _model.LegalEntityId = 0;

            //Act
            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(_model, ExpectedUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<AddPayeToNewLegalEntityCommand>(c => c.AccountId.Equals(ExpectedAccountId) && c.Empref.Equals(ExpectedEmpref) && c.ExternalUserId.Equals(ExpectedUserId) && c.LegalEntityCode.Equals(_model.LegalEntityCode))), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<AddPayeToAccountForExistingLegalEntityCommand>()), Times.Never);
        }
    }
}
