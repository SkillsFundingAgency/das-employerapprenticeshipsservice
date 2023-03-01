using System.Data;
using MediatR;
using SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerAccounts.Queries.GetGatewayToken;
using SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerAccounts.Queries.GetMember;
using SFA.DAS.Encoding;
using SFA.DAS.NLog.Logger;
using EmpRefLevyInformation = HMRC.ESFA.Levy.Api.Types.EmpRefLevyInformation;
using Name = HMRC.ESFA.Levy.Api.Types.Name;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenAddingAPayeScheme
    {
        private EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private ConfirmNewPayeSchemeViewModel _model;
        private EmployerAccountsConfiguration _configuration;
        private const string ExpectedHashedId = "jgdfg786";
        private const string ExpectedEmpref = "123/DFDS";
        private const string ExpectedEmprefName = "Paye Scheme 1";
        private const string ExpectedUserId = "someid";

        [SetUp]
        public void Arrange()
        {
            _model = new ConfirmNewPayeSchemeViewModel
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                HashedAccountId = ExpectedHashedId,
                PayeScheme = ExpectedEmpref,
                PayeName = ExpectedEmprefName
            };

            _configuration = new EmployerAccountsConfiguration { Hmrc = new HmrcConfiguration() };

            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountLegalEntitiesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetAccountLegalEntitiesResponse { LegalEntities = new List<AccountSpecificLegalEntity>() });
            _mediator.Setup(x => x.Send(It.Is<GetGatewayTokenQuery>(c => c.AccessCode.Equals("1")), It.IsAny<CancellationToken>())).ReturnsAsync(new GetGatewayTokenQueryResponse { HmrcTokenResponse = new HmrcTokenResponse { AccessToken = "1" } });
            _mediator.Setup(x => x.Send(It.Is<GetHmrcEmployerInformationQuery>(c => c.AuthToken.Equals("1")), It.IsAny<CancellationToken>())).ReturnsAsync(new GetHmrcEmployerInformationResponse { Empref = "123/ABC", EmployerLevyInformation = new EmpRefLevyInformation { Employer = new HMRC.ESFA.Levy.Api.Types.Employer { Name = new Name { EmprefAssociatedName = ExpectedEmprefName } } } });
            _mediator.Setup(x => x.Send(It.Is<GetHmrcEmployerInformationQuery>(c => c.AuthToken.Equals("2")), It.IsAny<CancellationToken>())).ReturnsAsync(new GetHmrcEmployerInformationResponse { Empref = "456/ABC", EmployerLevyInformation = new EmpRefLevyInformation { Employer = new HMRC.ESFA.Levy.Api.Types.Employer { Name = new Name { EmprefAssociatedName = ExpectedEmprefName } } } });

            _employerAccountPayeOrchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _cookieService.Object, _configuration, Mock.Of<IEncodingService>());
        }
        
        [Test]
        public async Task ThenTheAddPayeToAccountCommandIsCalled()
        {
            //Act
            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(_model, ExpectedUserId);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<AddPayeToAccountCommand>(c => c.HashedAccountId.Equals(ExpectedHashedId) && c.Empref.Equals(ExpectedEmpref) && c.ExternalUserId.Equals(ExpectedUserId) && c.EmprefName.Equals(ExpectedEmprefName)), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Test]
        public async Task ThenTheCallToHmrcIsPerformed()
        {
            //Act
            await _employerAccountPayeOrchestrator.GetPayeConfirmModel("1", "1", "", null);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<GetHmrcEmployerInformationQuery>(c => c.AuthToken.Equals("1")), It.IsAny<CancellationToken>()), Times.Once);
        }
        

        [Test]
        public async Task ThenIfTheSchemeExistsAConflictIsReturnedAndTheValuesAreCleared()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetHmrcEmployerInformationQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ConstraintException());
            
            //Act
            var actual = await _employerAccountPayeOrchestrator.GetPayeConfirmModel("1", "1", "", null);

            //Assert
            Assert.IsEmpty(actual.Data.PayeScheme);
            Assert.IsEmpty(actual.Data.AccessToken);
            Assert.IsEmpty(actual.Data.RefreshToken);
            Assert.IsEmpty(actual.Data.PayeName);
        }

        [Test]
        public async Task ThenTheLoggedInUserIsCheckedToMakeSureThatTheyAreAnOwner()
        {
            //Act
            await _employerAccountPayeOrchestrator.CheckUserIsOwner(ExpectedHashedId, ExpectedUserId, "", "");

            //assert
            _mediator.Verify(x => x.Send(It.IsAny<GetMemberRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenIfNotAuthorisedItIsReturnedInTheResponse()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetMemberRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetMemberResponse { TeamMember = new TeamMember { Role = Role.Viewer } });

            //Act
            var actual = await _employerAccountPayeOrchestrator.CheckUserIsOwner(ExpectedHashedId, ExpectedUserId, "", "");

            //act
            Assert.IsAssignableFrom<OrchestratorResponse<GatewayInformViewModel>>(actual);
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }
    }
}
