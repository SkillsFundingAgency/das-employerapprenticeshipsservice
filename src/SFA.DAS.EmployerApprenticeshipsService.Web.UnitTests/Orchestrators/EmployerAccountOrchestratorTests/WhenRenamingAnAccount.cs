using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Entities.Account;
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
            
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountQuery>()))
                .ReturnsAsync(new GetEmployerAccountResponse {Account = _account});

            _orchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenTheCorrectAccountDetailsShouldBeReturned()
        {
            //Act
            var response = await _orchestrator.GetEmployerAccount("123ABC");
            
            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetEmployerAccountQuery>(q => q.AccountId.Equals(_account.HashedId))));
            Assert.AreEqual(_account.HashedId, response.Data.HashedId);
            Assert.AreEqual(_account.Name, response.Data.Name);
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }
    }
}
