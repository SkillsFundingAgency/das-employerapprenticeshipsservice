﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    [TestFixture]
    public class WhenIGetTheTransactionSummaryForAnAccount : AccountTransactionsControllerTests
    {
        private AccountTransactionsController _controller;             
        private Mock<ILogger<AccountTransactionsOrchestrator>> _logger;
        private Mock<IUrlHelper> _urlHelper;
        private  Mock<IEmployerFinanceApiService> _financeApiService;
        protected IMapper _mapper;        

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger<AccountTransactionsOrchestrator>>();
            _urlHelper = new Mock<IUrlHelper>();            
            _financeApiService = new Mock<IEmployerFinanceApiService>();
            _mapper = ConfigureMapper();
            var orchestrator = new AccountTransactionsOrchestrator(_logger.Object, _financeApiService.Object);
            _controller = new AccountTransactionsController(orchestrator);
            _controller.Url = _urlHelper.Object; 
        }

        //[Test]
        //public async Task ThenTheTransactionSummaryIsReturned()
        //{
        //    //Arrange
        //    var hashedAccountId = "ABC123";           
        //    var fixture = new Fixture();
        //    var apiResponse = new List<TransactionSummaryViewModel>()
        //    {
        //         fixture.Create<TransactionSummaryViewModel>(),
        //         fixture.Create<TransactionSummaryViewModel>()
        //    };

        //    _financeApiService.Setup(x => x.GetTransactionSummary(hashedAccountId)).ReturnsAsync(apiResponse);

        //    var firstExpectedUri = "someuri";
        //    _urlHelper.Setup(
        //            x =>
        //                x.Link("GetTransactions",
        //                    It.Is<object>(o => o.Is()..IsNot(new { hashedAccountId, year = apiResponse.First().Year, month = apiResponse.First().Month }))))
        //        .Returns(firstExpectedUri);
        //    var secondExpectedUri = "someotheruri";
        //    _urlHelper.Setup(
        //            x =>
        //                x.Link("GetTransactions",
        //                    It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = apiResponse.Last().Year, month = apiResponse.Last().Month }))))
        //        .Returns(secondExpectedUri);

        //    //Act
        //    var response = await _controller.Index(hashedAccountId);

            
        //    //Assert            
        //    Assert.IsNotNull(response);
        //    Assert.IsInstanceOf<OkObjectResult>(response);
        //    var okModel = response as OkObjectResult;

        //    Assert.IsInstanceOf<AccountResourceList<TransactionSummaryViewModel>>(okModel.Value);
        //    var model = okModel.Value as AccountResourceList<TransactionSummaryViewModel>;

        //    model?.Should().NotBeNull();
        //    model?.ShouldAllBeEquivalentTo(apiResponse, x => x.Excluding(y => y.Href));
        //    model?.First().Href.Should().Be(firstExpectedUri);
        //    model?.Last().Href.Should().Be(secondExpectedUri);
        //}
    }
}
