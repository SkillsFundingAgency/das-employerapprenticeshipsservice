using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeWithExistingLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayToken;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetMember;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
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
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private const long ExpectedAccountId = 73636363;
        private const string ExpectedHashedId = "jgdfg786";
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

            _configuration = new EmployerApprenticeshipsServiceConfiguration();
            _configuration.Hmrc = new HmrcConfiguration {IgnoreDuplicates = true};

            _logger = new Mock<ILogger>();
            _cookieService = new Mock<ICookieService>();
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>())).ReturnsAsync(new GetAccountLegalEntitiesResponse{Entites = new LegalEntities {LegalEntityList = new List<LegalEntity>()}});
            _mediator.Setup(x => x.SendAsync(It.Is<GetGatewayTokenQuery>(c=>c.AccessCode.Equals("1")))).ReturnsAsync(new GetGatewayTokenQueryResponse {HmrcTokenResponse = new HmrcTokenResponse {AccessToken = "1"} });
            _mediator.Setup(x => x.SendAsync(It.Is<GetHmrcEmployerInformationQuery>(c=>c.AuthToken.Equals("1")))).ReturnsAsync(new GetHmrcEmployerInformationResponse {Empref = "123/ABC"});
            _mediator.Setup(x => x.SendAsync(It.Is<GetHmrcEmployerInformationQuery>(c=>c.AuthToken.Equals("2")))).ReturnsAsync(new GetHmrcEmployerInformationResponse {Empref = "456/ABC"});

            _employerAccountPayeOrchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object,_logger.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledWithTheGetAccountLegalEntitesQuery()
        {
            //Arrange
            var expectedUserId = Guid.NewGuid().ToString();

            //Act
            await _employerAccountPayeOrchestrator.GetLegalEntities(ExpectedHashedId, expectedUserId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(c=>c.HashedId.Equals(ExpectedHashedId) && c.UserId.Equals(expectedUserId))));
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

        [Test]
        public async Task ThenTheCallToHmrcIsPerformedIfWeAreCheckingDuplicates()
        {
            //Arrange
            _configuration.Hmrc = new HmrcConfiguration { IgnoreDuplicates = false };

            //Act
            await _employerAccountPayeOrchestrator.GetPayeConfirmModel(1, "1", "", null);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetHmrcEmployerInformationQuery>(c=>c.AuthToken.Equals("1"))), Times.Once);
        }
        

        [Test]
        public async Task ThenIfTheSchemeExistsAConflictIsReturnedAndTheValuesAreCleared()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetHmrcEmployerInformationQuery>())).ThrowsAsync(new ConstraintException());
            _configuration.Hmrc = new HmrcConfiguration { IgnoreDuplicates = false };
            
            //Act
            var actual = await _employerAccountPayeOrchestrator.GetPayeConfirmModel(1, "1", "", null);

            //Assert
            Assert.IsEmpty(actual.Data.PayeScheme);
            Assert.IsEmpty(actual.Data.AccessToken);
            Assert.IsEmpty(actual.Data.RefreshToken);
        }

        [Test]
        public async Task ThenTheLoggedInUserIsCheckedToMakeSureThatTheyAreAnOwner()
        {
            //Act
            await _employerAccountPayeOrchestrator.CheckUserIsOwner(ExpectedHashedId, ExpectedUserId, "");

            //assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetMemberRequest>()), Times.Once);
        }

        [Test]
        public async Task ThenIfNotAuthorisedItIsReturnedInTheResponse()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetMemberRequest>())).ReturnsAsync(new GetMemberResponse {TeamMember = new TeamMember {Role=Role.Viewer} });

            //Act
            var actual = await _employerAccountPayeOrchestrator.CheckUserIsOwner(ExpectedHashedId, ExpectedUserId,"");

            //act
            Assert.IsAssignableFrom<OrchestratorResponse<BeginNewPayeScheme>>(actual);
            Assert.AreEqual(HttpStatusCode.Unauthorized,actual.Status);
        }
    }
}
