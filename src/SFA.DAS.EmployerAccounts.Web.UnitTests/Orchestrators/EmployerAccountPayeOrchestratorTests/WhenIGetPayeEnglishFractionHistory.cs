using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.Levy;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Hmrc.Configuration;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenIGetPayeEnglishFractionHistory
    {
        private EmployerAccountsConfiguration _configuration;
        private Mock<ILog> _logger;
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

            _logger = new Mock<ILog>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerEnglishFractionHistoryQuery>())).ReturnsAsync(new GetEmployerEnglishFractionHistoryResponse { Fractions = new List<DasEnglishFraction>() });
            
            _employerAccountPayeOrchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetTheEnglishFractionHistory()
        {
            //Act
            await _employerAccountPayeOrchestrator.GetPayeDetails(EmpRef, AccountId, UserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetEmployerEnglishFractionHistoryQuery>()), Times.Once);
        }

        [Test]
        public async Task TheIfTheCallReturnsAnUnauthorizedAccessExceptionTheResponseIsSetAsUnauthenticated()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerEnglishFractionHistoryQuery>())).ThrowsAsync(new UnauthorizedAccessException(""));

            //Act
            var actual = await _employerAccountPayeOrchestrator.GetPayeDetails(EmpRef, AccountId, UserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }
    }
}
