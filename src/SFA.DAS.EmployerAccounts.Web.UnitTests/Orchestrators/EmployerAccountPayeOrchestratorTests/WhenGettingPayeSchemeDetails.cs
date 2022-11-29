using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.Levy;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Hmrc.Configuration;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenGettingPayeSchemeDetails
    {
        private const string SchemeName = "Test Scheme";
        private const string EmpRef = "123/AGB";
        private const string AccountId = "123aBB";
        private const string UserId = "45AGB22";

        private EmployerAccountsConfiguration _configuration;
        private Mock<ILog> _logger;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private Mock<IMediator> _mediator;
        private EmployerAccountPayeOrchestrator _orchestrator;
        private PayeSchemeView _payeScheme;

        [SetUp]
        public void Setup()
        {
            _configuration = new EmployerAccountsConfiguration { Hmrc = new HmrcConfiguration() };
            _logger = new Mock<ILog>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _mediator = new Mock<IMediator>();

            _payeScheme = new PayeSchemeView
            {
                Ref = EmpRef,
                Name = SchemeName,
                AddedDate = DateTime.Now
            };

            _mediator
                .Setup(x => x.SendAsync(It.IsAny<GetEmployerEnglishFractionHistoryQuery>()))
                .ReturnsAsync(new GetEmployerEnglishFractionHistoryResponse { Fractions = new List<DasEnglishFraction>() });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPayeSchemeByRefQuery>()))
                .ReturnsAsync(new GetPayeSchemeByRefResponse
                {
                    PayeScheme = _payeScheme
                });

            _orchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetThePayeScheme()
        {
            //Act
            await _orchestrator.GetPayeDetails(EmpRef, AccountId, UserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetPayeSchemeByRefQuery>()), Times.Once);
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
