using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenIGetPayeEnglishFractionHistory
    {
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<ILogger> _logger;
        private Mock<ICookieService<EmployerAccountData>> _cookieService;
        private Mock<IMediator> _mediator;
        private EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;
        private const string EmpRef = "123/AGB";
        private const string AccountId = "123aBB";
        private const string UserId = "45AGB22";

        [SetUp]
        public void Arrange()
        {
            
            _configuration = new EmployerApprenticeshipsServiceConfiguration { Hmrc = new HmrcConfiguration() };

            _logger = new Mock<ILogger>();
            _cookieService = new Mock<ICookieService<EmployerAccountData>>();

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerEnglishFractionQuery>())).ReturnsAsync(new GetEmployerEnglishFractionResponse { Fractions = new List<DasEnglishFraction>()  });
            
            _employerAccountPayeOrchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetTheEnglishFractionHistory()
        {
            //Act
            await _employerAccountPayeOrchestrator.GetPayeDetails(EmpRef, AccountId, UserId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny<GetEmployerEnglishFractionQuery>()),Times.Once);
        }

        [Test]
        public async Task TheIfTheCallReturnsAnUnauthorizedAccessExceptionTheResponseIsSetAsUnauthenticated()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerEnglishFractionQuery>())).ThrowsAsync(new UnauthorizedAccessException(""));

            //Act
            var actual = await _employerAccountPayeOrchestrator.GetPayeDetails(EmpRef, AccountId, UserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status );
        }
    }
}
