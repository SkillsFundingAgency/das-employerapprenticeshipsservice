using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RenameEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.User;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests
{
    public  class WhenRenamingAnAccount
    {
        private Mock<ICookieService> _cookieService;
        private Mock<ILogger> _logger;
        private Mock<IMediator> _mediator;
        private EmployerAccountOrchestrator _orchestrator;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Account _account;

        [SetUp]
        public void Arrange()
        {
            _cookieService = new Mock<ICookieService>();
            _logger = new Mock<ILogger>();
            _mediator = new Mock<IMediator>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _account = new Account
            {
                Id = 123,
                HashedId = "ABC123",
                Name = "Test Account"
            };
            
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(new GetEmployerAccountResponse {Account = _account});

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse {UserRole = Role.Owner});

            _orchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenTheCorrectAccountDetailsShouldBeReturned()
        {
            //Act
            var response = await _orchestrator.GetEmployerAccount("ABC123");
            
            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetEmployerAccountHashedQuery>(q => q.HashedAccountId.Equals(_account.HashedId))));
            Assert.AreEqual(_account.HashedId, response.Data.HashedId);
            Assert.AreEqual(_account.Name, response.Data.Name);
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test]
        public async Task ThenTheAccountNameShouldBeUpdated()
        { 
            //Act
            var response = await _orchestrator.RenameEmployerAccount(new RenameEmployerAccountViewModel
            {
                NewName = "New Account Name"
            }, "ABC123");

            //Assert
            Assert.IsInstanceOf<OrchestratorResponse<RenameEmployerAccountViewModel>>(response);

            _mediator.Verify(x =>
                    x.SendAsync(It.Is<RenameEmployerAccountCommand>(c => c.NewName == "New Account Name")),
                    Times.Once());
        }
    }
}
