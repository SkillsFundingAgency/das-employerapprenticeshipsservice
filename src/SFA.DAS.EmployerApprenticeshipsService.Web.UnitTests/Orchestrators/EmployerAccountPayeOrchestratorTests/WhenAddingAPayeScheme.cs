using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.AddPayeToAccount;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetGatewayToken;
using SFA.DAS.EAS.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetMember;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenAddingAPayeScheme
    {
        private EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<ICookieService> _cookieService;
        private ConfirmNewPayeScheme _model;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private const string ExpectedHashedId = "jgdfg786";
        private const string ExpectedEmpref = "123/DFDS";
        private const string ExpectedEmprefName = "Paye Scheme 1";
        private const string ExpectedUserId = "someid";

        [SetUp]
        public void Arrange()
        {
            _model = new ConfirmNewPayeScheme
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                HashedAccountId = ExpectedHashedId,
                PayeScheme = ExpectedEmpref,
                PayeName = ExpectedEmprefName
            };

            _configuration = new EmployerApprenticeshipsServiceConfiguration {Hmrc = new HmrcConfiguration()};

            _logger = new Mock<ILogger>();
            _cookieService = new Mock<ICookieService>();
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>())).ReturnsAsync(new GetAccountLegalEntitiesResponse{Entites = new LegalEntities {LegalEntityList = new List<LegalEntity>()}});
            _mediator.Setup(x => x.SendAsync(It.Is<GetGatewayTokenQuery>(c=>c.AccessCode.Equals("1")))).ReturnsAsync(new GetGatewayTokenQueryResponse {HmrcTokenResponse = new HmrcTokenResponse {AccessToken = "1"} });
            _mediator.Setup(x => x.SendAsync(It.Is<GetHmrcEmployerInformationQuery>(c=>c.AuthToken.Equals("1")))).ReturnsAsync(new GetHmrcEmployerInformationResponse {Empref = "123/ABC", EmployerLevyInformation = new EmpRefLevyInformation {Employer = new Employer {Name = new Name {EmprefAssociatedName = ExpectedEmprefName} } } });
            _mediator.Setup(x => x.SendAsync(It.Is<GetHmrcEmployerInformationQuery>(c=>c.AuthToken.Equals("2")))).ReturnsAsync(new GetHmrcEmployerInformationResponse {Empref = "456/ABC", EmployerLevyInformation = new EmpRefLevyInformation { Employer = new Employer { Name = new Name { EmprefAssociatedName = ExpectedEmprefName } } } });

            _employerAccountPayeOrchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object,_logger.Object, _cookieService.Object, _configuration);
        }
        

        [Test]
        public async Task ThenTheAddPayeToAccountCommandIsCalled()
        {
            //Act
            await _employerAccountPayeOrchestrator.AddPayeSchemeToAccount(_model, ExpectedUserId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<AddPayeToAccountCommand>(c=>c.HashedAccountId.Equals(ExpectedHashedId) && c.Empref.Equals(ExpectedEmpref) && c.ExternalUserId.Equals(ExpectedUserId) && c.EmprefName.Equals(ExpectedEmprefName) )),Times.Once);
        }
        

        [Test]
        public async Task ThenTheCallToHmrcIsPerformed()
        {
            //Act
            await _employerAccountPayeOrchestrator.GetPayeConfirmModel("1", "1", "", null);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetHmrcEmployerInformationQuery>(c=>c.AuthToken.Equals("1"))), Times.Once);
        }
        

        [Test]
        public async Task ThenIfTheSchemeExistsAConflictIsReturnedAndTheValuesAreCleared()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetHmrcEmployerInformationQuery>())).ThrowsAsync(new ConstraintException());
            
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
            await _employerAccountPayeOrchestrator.CheckUserIsOwner(ExpectedHashedId, ExpectedUserId, "","","");

            //assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetMemberRequest>()), Times.Once);
        }

        [Test]
        public async Task ThenIfNotAuthorisedItIsReturnedInTheResponse()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetMemberRequest>())).ReturnsAsync(new GetMemberResponse {TeamMember = new TeamMember {Role=Role.Viewer} });

            //Act
            var actual = await _employerAccountPayeOrchestrator.CheckUserIsOwner(ExpectedHashedId, ExpectedUserId,"","","");

            //act
            Assert.IsAssignableFrom<OrchestratorResponse<GatewayInformViewModel>>(actual);
            Assert.AreEqual(HttpStatusCode.Unauthorized,actual.Status);
        }
    }
}
