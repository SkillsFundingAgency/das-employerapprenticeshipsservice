using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountTransactionsControllerTests;

[TestFixture]
public class WhenIGetTransactionsForAnAccount : AccountTransactionsControllerTests
{
    private AccountTransactionsController? _controller;        
    private Mock<ILogger<AccountTransactionsOrchestrator>>? _logger;
    private Mock<IUrlHelper>? _urlHelper;        
    private Mock<IEmployerFinanceApiService>? _financeApiService;
    private TransactionsViewModel? _transactionsViewModel;

    [SetUp]
    public void Arrange()
    {           
        _logger = new Mock<ILogger<AccountTransactionsOrchestrator>>();
        _urlHelper = new Mock<IUrlHelper>();
        _urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("dummyurl");            
        _financeApiService = new Mock<IEmployerFinanceApiService>();
        var orchestrator = new AccountTransactionsOrchestrator(_logger.Object, _financeApiService.Object);
        _controller = new AccountTransactionsController(orchestrator)
        {
            Url = _urlHelper.Object
        };
        _transactionsViewModel = new TransactionsViewModel
        {
            new() { Description = "Is Not Null", Amount = 100m, DateCreated = DateTime.Today },
            new() { Description = "Is Not Null 2", Amount = 100m, DateCreated = DateTime.Today }
        };
    }

    [Test]
    public async Task ThenTheTransactionsAreReturned()
    {            
        //Arrange
        const string hashedAccountId = "ABC123";
        const int year = 2017;
        const int month = 3;            
        _financeApiService!.Setup(x => x.GetTransactions(hashedAccountId, year, month, CancellationToken.None)).ReturnsAsync(_transactionsViewModel);

        //Act
        var response = await _controller!.GetTransactions(hashedAccountId, year, month);
                        
        //Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<ActionResult<TransactionsViewModel>>());
        var model = response as ActionResult<TransactionsViewModel>;

        Assert.That(model.Result, Is.Not.Null);
        Assert.That(model.Result, Is.InstanceOf<OkObjectResult>());

        var oKResult = model.Result as OkObjectResult;

        Assert.That(oKResult!.Value, Is.Not.Null);
        Assert.That(oKResult.Value, Is.InstanceOf<TransactionsViewModel>());

        var value = oKResult.Value as TransactionsViewModel;

        value.Should().NotBeNull();
    }

    [Test]
    public async Task AndThereAreNoPreviousTransactionThenTheUrlIsNotSet()
    {
        //Arrange
        const string hashedAccountId = "ABC123";
        const int year = 2017;
        const int month = 3;          
        _financeApiService!.Setup(x => x.GetTransactions(hashedAccountId, year, month, CancellationToken.None)).ReturnsAsync(_transactionsViewModel);
            
        //Act
        var response = await _controller!.GetTransactions(hashedAccountId, year, month);
            
        //Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<ActionResult<TransactionsViewModel>>());
        var model = response as ActionResult<TransactionsViewModel>;

        Assert.That(model.Result, Is.Not.Null);
        Assert.That(model.Result, Is.InstanceOf<OkObjectResult>());

        var oKObject = model.Result as OkObjectResult;

        Assert.That(oKObject!.Value, Is.Not.Null);
        Assert.That(oKObject.Value, Is.InstanceOf<TransactionsViewModel>());

        var value = oKObject.Value as TransactionsViewModel;

        value.Should().NotBeNull();
        value!.PreviousMonthUri.Should().BeNullOrEmpty();
        _urlHelper!.Verify(x => x.Link("GetTransactions", It.IsAny<object>()), Times.Never);
    }

    [Test]
    public async Task AndNoMonthIsProvidedThenTheCurrentMonthIsUsed()
    {
        //Arrange
        const string hashedAccountId = "ABC123";
        const int year = 2017;                    
        _transactionsViewModel!.HasPreviousTransactions = false;
        _transactionsViewModel.Year = year;            

        _financeApiService!.Setup(x => x.GetTransactions(hashedAccountId, year, DateTime.Now.Month, CancellationToken.None)).ReturnsAsync(_transactionsViewModel);

        //Act
        var response = await _controller!.GetTransactions(hashedAccountId, year);

        //Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<ActionResult<TransactionsViewModel>>());
        var model = response as ActionResult<TransactionsViewModel>;

        Assert.That(model.Result, Is.Not.Null);
        Assert.That(model.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = model.Result as OkObjectResult;

        Assert.That(okResult!.Value, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<TransactionsViewModel>());

        var value = okResult.Value as TransactionsViewModel;

        value.Should().NotBeNull();
        value!.PreviousMonthUri.Should().BeNullOrEmpty();
        _urlHelper!.Verify(x => x.Link("GetTransactions", It.IsAny<object>()), Times.Never);
    }

    [Test]
    public async Task AndNoYearIsProvidedThenTheCurrentYearIsUsed()
    {
        const string hashedAccountId = "ABC123";          
        _financeApiService!.Setup(x => x.GetTransactions(hashedAccountId, DateTime.Now.Year, DateTime.Now.Month, CancellationToken.None)).ReturnsAsync(_transactionsViewModel);

        //Act
        var response = await _controller!.GetTransactions(hashedAccountId);
            
        //Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<ActionResult<TransactionsViewModel>>());
        var model = response as ActionResult<TransactionsViewModel>;

        Assert.That(model.Result, Is.Not.Null);
        Assert.That(model.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = model.Result as OkObjectResult;

        Assert.That(okResult!.Value, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<TransactionsViewModel>());

        var value = okResult.Value as TransactionsViewModel;

        value.Should().NotBeNull();
    }
}