using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.AccountsOrchestratorTests;

internal class WhenIGetAnAccount
{
    private AccountsOrchestrator? _orchestrator;
    private Mock<ILogger<AccountsOrchestrator>>? _log;
    private Mock<IEncodingService>? _encodingService;
    private Mock<IEmployerAccountsApiService>? _apiService;
    private Mock<IEmployerFinanceApiService>? _financeApiService;
    private IMapper? _mapper;
    private TransferAllowance? _transferAllowance;

    private const decimal AccountBalance = 678.90M;
    private const string HashedAgreementId = "ABC123";

    private AccountDetailViewModel? _accountDetailViewModel;
    private AccountBalance? _accountBalanceResult;

    [SetUp]
    public void Arrange()
    {
        _transferAllowance = new TransferAllowance { RemainingTransferAllowance = 123.45M, StartingTransferAllowance = 234.56M };            
        _mapper = ConfigureMapper();
        _log = new Mock<ILogger<AccountsOrchestrator>>();
        _encodingService = new Mock<IEncodingService>();
        _apiService = new Mock<IEmployerAccountsApiService>();
        _financeApiService = new Mock<IEmployerFinanceApiService>();
        _orchestrator = new AccountsOrchestrator(_log.Object, _mapper, _encodingService.Object, _apiService.Object, _financeApiService.Object);
        
        _accountDetailViewModel = new AccountDetailViewModel { AccountId = 1, ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString() };

        _apiService
            .Setup(x => x.GetAccount("ABC123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(_accountDetailViewModel)
            .Verifiable("Get account was not called");

        var transferAllowance = _transferAllowance;            
        _financeApiService.Setup(x => x.GetTransferAllowance(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(transferAllowance);

        _accountBalanceResult = new AccountBalance { Balance = AccountBalance };
        var accountBalances = new List<AccountBalance?> { _accountBalanceResult };
        _financeApiService.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>(), CancellationToken.None)).ReturnsAsync(accountBalances);
    }

    [Test]
    public async Task ThenARequestShouldMakeCallsToGetBalances()
    {
        //Act
        await _orchestrator!.GetAccount(HashedAgreementId);

        //Assert            
        _apiService!.VerifyAll();
        _financeApiService!.VerifyAll();
    }

    [Test]
    public async Task ThenResponseShouldHaveBalanceSet()
    {
        //Act
        var result = await _orchestrator!.GetAccount(HashedAgreementId);

        //Assert
        Assert.That(result.Data.Balance, Is.EqualTo(AccountBalance));
    }

    [Test]
    public async Task ThenResponseShouldHaveTransferAllowanceSet()
    {
        //Act
        var result = await _orchestrator!.GetAccount(HashedAgreementId);

        //Assert
        Assert.That(result.Data.RemainingTransferAllowance, Is.EqualTo(_transferAllowance!.RemainingTransferAllowance));
    }

    [Test]
    public async Task AndTheAccountHasALevyOverrideThenTheyShouldNotBeAllowedPaymentOnTheService()
    {
        _accountBalanceResult!.LevyOverride = false;

        var response = await _orchestrator!.GetAccount(HashedAgreementId);

        Assert.That(response.Data.IsAllowedPaymentOnService, Is.False);
    }

    [Test]
    public async Task AndTheAccountHasAnEoIAgreementThenTheyShouldBeAllowedPaymentOnTheService()
    {
        _accountDetailViewModel!.AccountAgreementType = AccountAgreementType.NonLevyExpressionOfInterest;

        var response = await _orchestrator!.GetAccount(HashedAgreementId);

        Assert.That(response.Data.IsAllowedPaymentOnService, Is.True);
    }

    [Test]
    public async Task AndTheEmployerTypeIsLevyThenTheyShouldBeAllowedPaymentOnTheService()
    {
        _accountDetailViewModel!.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy.ToString();

        var response = await _orchestrator!.GetAccount(HashedAgreementId);

        Assert.That(response.Data.IsAllowedPaymentOnService, Is.True);
    }

    [Test]
    public async Task AndTheEmployerTypeIsNoneLevyThenTheyShouldNotBeAllowedPaymentOnTheService()
    {
        _accountDetailViewModel!.ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy.ToString();

        var response = await _orchestrator!.GetAccount(HashedAgreementId);

        Assert.That(response.Data.IsAllowedPaymentOnService, Is.False);
    }

    private static IMapper? ConfigureMapper()
    {
        var profiles = Assembly.Load("SFA.DAS.EAS.Account.Api")
            .GetTypes()
            .Where(type => typeof(Profile).IsAssignableFrom(type))
            .Select(type => (Profile)Activator.CreateInstance(type)!)
            .ToList();

        var config = new MapperConfiguration(c =>
        {
            profiles.ForEach(c.AddProfile);
        });

        return config.CreateMapper();
    }
}