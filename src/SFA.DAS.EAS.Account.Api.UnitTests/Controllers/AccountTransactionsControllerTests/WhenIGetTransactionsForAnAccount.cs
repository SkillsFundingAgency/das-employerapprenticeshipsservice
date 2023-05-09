using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    [TestFixture]
    public class WhenIGetTransactionsForAnAccount : AccountTransactionsControllerTests
    {
        private AccountTransactionsController _controller;        
        private Mock<ILogger<AccountTransactionsOrchestrator>> _logger;
        private Mock<IUrlHelper> _urlHelper;        
        private Mock<IEmployerFinanceApiService> _financeApiService;
        protected IMapper _mapper;
        private TransactionsViewModel transactionsViewModel;

        [SetUp]
        public void Arrange()
        {           
            _logger = new Mock<ILogger<AccountTransactionsOrchestrator>>();
            _urlHelper = new Mock<IUrlHelper>();
            _urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("dummyurl");            
            _financeApiService = new Mock<IEmployerFinanceApiService>();
            _mapper = ConfigureMapper();
            var orchestrator = new AccountTransactionsOrchestrator(_logger.Object, _financeApiService.Object);
            _controller = new AccountTransactionsController(orchestrator);
            _controller.Url = _urlHelper.Object;
            transactionsViewModel = new TransactionsViewModel
            {
                new TransactionViewModel  { Description = "Is Not Null", Amount = 100m, DateCreated = DateTime.Today },
                new TransactionViewModel  { Description = "Is Not Null 2", Amount = 100m, DateCreated = DateTime.Today }
            };
        }

        [Test]
        public async Task ThenTheTransactionsAreReturned()
        {            
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 3;            
            _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, year, month)).ReturnsAsync(transactionsViewModel);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year, month);
                        
            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ActionResult<TransactionsViewModel>>(response);
            var model = response as ActionResult<TransactionsViewModel>;

            Assert.IsNotNull(model.Result);
            Assert.IsInstanceOf<OkObjectResult>(model.Result);

            var oKResult = model.Result as OkObjectResult;

            Assert.IsNotNull(oKResult.Value);
            Assert.IsInstanceOf<TransactionsViewModel>(oKResult.Value);

            var value = oKResult.Value as TransactionsViewModel;

            value.Should().NotBeNull();
        }

        [Test]
        public async Task AndThereAreNoPreviousTransactionThenTheUrlIsNotSet()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 3;          
            _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, year, month)).ReturnsAsync(transactionsViewModel);
            
            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year, month);
            
            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ActionResult<TransactionsViewModel>>(response);
            var model = response as ActionResult<TransactionsViewModel>;

            Assert.IsNotNull(model.Result);
            Assert.IsInstanceOf<OkObjectResult>(model.Result);

            var oKObject = model.Result as OkObjectResult;

            Assert.IsNotNull(oKObject.Value);
            Assert.IsInstanceOf<TransactionsViewModel>(oKObject.Value);

            var value = oKObject.Value as TransactionsViewModel;

            value.Should().NotBeNull();
            value.PreviousMonthUri.Should().BeNullOrEmpty();
            _urlHelper.Verify(x => x.Link("GetTransactions", It.IsAny<object>()), Times.Never);
        }

        //[Test]
        //public async Task AndThereArePreviousTransactionsThenTheLinkIsCorrect()
        //{
        //    //Arrange
        //    var hashedAccountId = "ABC123";
        //    var year = 2017;
        //    var month = 1;                
        //    transactionsViewModel.HasPreviousTransactions = true;
        //    transactionsViewModel.Year = year;
        //    transactionsViewModel.Month = month;

        //    _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, year, month)).ReturnsAsync(transactionsViewModel);

        //    var expectedUri = "someuri";
        //    _urlHelper.Setup(x => x.Link("GetTransactions", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = year - 1, month = 12 })))).Returns(expectedUri);

        //    //Act
        //    var response = await _controller.GetTransactions(hashedAccountId, year, month);
        //    var model = response as ActionResult<TransactionsViewModel>;

        //    Assert.IsNotNull(model.Result);
        //    Assert.IsInstanceOf<OkObjectResult>(model.Result);

        //    var oKResult = model.Result as OkObjectResult;

        //    Assert.IsNotNull(oKResult.Value);
        //    Assert.IsInstanceOf<TransactionsViewModel>(oKResult.Value);

        //    var value = oKResult.Value as TransactionsViewModel;

        //    //Assert
        //    value.PreviousMonthUri.Should().Be(expectedUri);
        //}

        [Test]
        public async Task AndNoMonthIsProvidedThenTheCurrentMonthIsUsed()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;                    
            transactionsViewModel.HasPreviousTransactions = false;
            transactionsViewModel.Year = year;            

            _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, year, DateTime.Now.Month)).ReturnsAsync(transactionsViewModel);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ActionResult<TransactionsViewModel>>(response);
            var model = response as ActionResult<TransactionsViewModel>;

            Assert.IsNotNull(model.Result);
            Assert.IsInstanceOf<OkObjectResult>(model.Result);

            var okResult = model.Result as OkObjectResult;

            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOf<TransactionsViewModel>(okResult.Value);

            var value = okResult.Value as TransactionsViewModel;

            value.Should().NotBeNull();
            value.PreviousMonthUri.Should().BeNullOrEmpty();
            _urlHelper.Verify(x => x.Link("GetTransactions", It.IsAny<object>()), Times.Never);
        }

        //[Test]
        //public async Task AndThereAreLevyTransactionsThenTheLinkIsCorrect()
        //{
        //    //Arrange
        //    var hashedAccountId = "ABC123";
        //    var year = 2017;
        //    var month = 1;
        //    var transactionsViewModel = new TransactionsViewModel
        //    {
        //        new TransactionViewModel  { Description = "Is Not Null", Amount = 100m, DateCreated =  DateTime.Today, ResourceUri = "someuri" },
        //        new TransactionViewModel  { Description = "Is Not Null 2", Amount = 100m, DateCreated =  DateTime.Today  }
        //    };
        //    transactionsViewModel.HasPreviousTransactions = true;
        //    transactionsViewModel.Year = year;
        //    transactionsViewModel.Month = month;

        //    _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, year, month)).ReturnsAsync(transactionsViewModel);

        //    var expectedUri = "someuri";
        //    _urlHelper.Setup(
        //            x =>
        //                x.Link("GetLevyForPeriod",
        //                    It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payrollYear = levyTransaction.PayrollYear, payrollMonth = levyTransaction.PayrollMonth }))))
        //        .Returns(expectedUri);

        //    //Act
        //    var response = await _controller.GetTransactions(hashedAccountId, year, month);
        //    var model = response as ActionResult<TransactionsViewModel>;

        //    Assert.IsNotNull(model.Result);
        //    Assert.IsInstanceOf<OkObjectResult>(model.Result);

        //    var oKResult = model.Result as OkObjectResult;

        //    Assert.IsNotNull(oKResult.Value);
        //    Assert.IsInstanceOf<TransactionsViewModel>(oKResult.Value);

        //    var value = oKResult.Value as TransactionsViewModel;

        //    //Assert
        //    value[0].ResourceUri.Should().Be(expectedUri);
        //}

        [Test]
        public async Task AndNoYearIsProvidedThenTheCurrentYearIsUsed()
        {
            var hashedAccountId = "ABC123";          
            _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, DateTime.Now.Year, DateTime.Now.Month)).ReturnsAsync(transactionsViewModel);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId);
            
            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ActionResult<TransactionsViewModel>>(response);
            var model = response as ActionResult<TransactionsViewModel>;

            Assert.IsNotNull(model.Result);
            Assert.IsInstanceOf<OkObjectResult>(model.Result);

            var okResult = model.Result as OkObjectResult;

            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOf<TransactionsViewModel>(okResult.Value);

            var value = okResult.Value as TransactionsViewModel;

            value.Should().NotBeNull();
        }
    }
}
