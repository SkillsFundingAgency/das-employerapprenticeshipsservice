using MediatR;
using SFA.DAS.EmployerAccounts.Models.Levy;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenIGetPayeEnglishFractionHistory
    {
        private EmployerAccountsConfiguration _configuration;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private Mock<IMediator> _mediator;
        private EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;
        private const string EmpRef = "123/AGB";
        private const string AccountId = "123aBB";
        private const string UserId = "45AGB22";

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerAccountsConfiguration { Hmrc = new HmrcConfiguration() };
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetEmployerEnglishFractionHistoryQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetEmployerEnglishFractionHistoryResponse { Fractions = new List<DasEnglishFraction>() });

            _employerAccountPayeOrchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _cookieService.Object, _configuration, Mock.Of<IEncodingService>());
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetTheEnglishFractionHistory()
        {
            //Act
            await _employerAccountPayeOrchestrator.GetPayeDetails(EmpRef, AccountId, UserId);

            //Assert
            _mediator.Verify(x => x.Send(It.IsAny<GetEmployerEnglishFractionHistoryQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task TheIfTheCallReturnsAnUnauthorizedAccessExceptionTheResponseIsSetAsUnauthenticated()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetEmployerEnglishFractionHistoryQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new UnauthorizedAccessException(""));

            //Act
            var actual = await _employerAccountPayeOrchestrator.GetPayeDetails(EmpRef, AccountId, UserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }
    }
}
