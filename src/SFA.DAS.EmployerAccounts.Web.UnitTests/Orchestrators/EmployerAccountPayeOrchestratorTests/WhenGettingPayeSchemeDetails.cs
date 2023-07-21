using MediatR;
using SFA.DAS.EmployerAccounts.Models.Levy;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenGettingPayeSchemeDetails
    {
        private const string SchemeName = "Test Scheme";
        private const string EmpRef = "123/AGB";
        private const string AccountId = "123aBB";
        private const string UserId = "45AGB22";

        private EmployerAccountsConfiguration _configuration;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private Mock<IMediator> _mediator;
        private EmployerAccountPayeOrchestrator _orchestrator;
        private PayeSchemeView _payeScheme;

        [SetUp]
        public void Setup()
        {
            _configuration = new EmployerAccountsConfiguration { Hmrc = new HmrcConfiguration() };
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _mediator = new Mock<IMediator>();

            _payeScheme = new PayeSchemeView
            {
                Ref = EmpRef,
                Name = SchemeName,
                AddedDate = DateTime.Now
            };

            _mediator
                .Setup(x => x.Send(It.IsAny<GetEmployerEnglishFractionHistoryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetEmployerEnglishFractionHistoryResponse { Fractions = new List<DasEnglishFraction>() });

            _mediator.Setup(x => x.Send(It.IsAny<GetPayeSchemeByRefQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetPayeSchemeByRefResponse
                {
                    PayeScheme = _payeScheme
                });

            _orchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _cookieService.Object, _configuration, Mock.Of<IEncodingService>());
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetThePayeScheme()
        {
            //Act
            await _orchestrator.GetPayeDetails(EmpRef, AccountId, UserId);

            //Assert
            _mediator.Verify(x => x.Send(It.IsAny<GetPayeSchemeByRefQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ShouldReturnPayeSchemeName()
        {
            //Act
            var result = await _orchestrator.GetPayeDetails(EmpRef, AccountId, UserId);

            //Assert
            Assert.AreEqual(SchemeName, result.Data.PayeSchemeName);
        }

        [Test]
        public async Task ShouldReturnEmptyPayeSchemeNameIfNoNameFound()
        {
            //Arrange
            _payeScheme.Name = null;

            //Act
            var result = await _orchestrator.GetPayeDetails(EmpRef, AccountId, UserId);

            //Assert
            Assert.IsEmpty(result.Data.PayeSchemeName);
        }
    }
}
