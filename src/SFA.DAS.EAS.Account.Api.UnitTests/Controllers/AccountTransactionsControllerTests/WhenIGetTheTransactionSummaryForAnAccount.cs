﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.TestCommon.Extensions;
using SFA.DAS.NLog.Logger;
using AutoMapper;
using AutoFixture;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    [TestFixture]
    public class WhenIGetTheTransactionSummaryForAnAccount : AccountTransactionsControllerTests
    {
        private AccountTransactionsController _controller;             
        private Mock<ILog> _logger;
        private Mock<UrlHelper> _urlHelper;
        private  Mock<IEmployerFinanceApiService> _financeApiService;
        protected IMapper _mapper;        

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILog>();
            _urlHelper = new Mock<UrlHelper>();            
            _financeApiService = new Mock<IEmployerFinanceApiService>();
            _mapper = ConfigureMapper();
            var orchestrator = new AccountTransactionsOrchestrator(_mapper, _logger.Object, _financeApiService.Object);
            _controller = new AccountTransactionsController(orchestrator);
            _controller.Url = _urlHelper.Object; 
        }

        [Test]
        public async Task ThenTheTransactionSummaryIsReturned()
        {
            //Arrange
            var hashedAccountId = "ABC123";           
            var fixture = new Fixture();
            var apiResponse = new List<TransactionSummaryViewModel>()
            {
                 fixture.Create<TransactionSummaryViewModel>(),
                 fixture.Create<TransactionSummaryViewModel>()
            };

            _financeApiService.Setup(x => x.GetTransactionSummary(hashedAccountId)).ReturnsAsync(apiResponse);

            var firstExpectedUri = "someuri";
            _urlHelper.Setup(
                    x =>
                        x.Route("GetTransactions",
                            It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = apiResponse.First().Year, month = apiResponse.First().Month }))))
                .Returns(firstExpectedUri);
            var secondExpectedUri = "someotheruri";
            _urlHelper.Setup(
                    x =>
                        x.Route("GetTransactions",
                            It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = apiResponse.Last().Year, month = apiResponse.Last().Month }))))
                .Returns(secondExpectedUri);

            //Act
            var response = await _controller.Index(hashedAccountId);

            
            //Assert            
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountResourceList<TransactionSummaryViewModel>>>(response);
            var model = response as OkNegotiatedContentResult<AccountResourceList<TransactionSummaryViewModel>>;

            model?.Content.Should().NotBeNull();
            model?.Content.ShouldAllBeEquivalentTo(apiResponse, x => x.Excluding(y => y.Href));
            model?.Content.First().Href.Should().Be(firstExpectedUri);
            model?.Content.Last().Href.Should().Be(secondExpectedUri);
        }
    }
}
