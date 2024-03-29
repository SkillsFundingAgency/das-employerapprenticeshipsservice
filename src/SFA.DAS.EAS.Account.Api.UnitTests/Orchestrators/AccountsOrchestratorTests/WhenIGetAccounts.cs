﻿using System.Globalization;
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
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.AccountsOrchestratorTests;

internal class WhenIGetAccounts
{
    private AccountsOrchestrator? _orchestrator;
    private Mock<ILogger<AccountsOrchestrator>>? _log;
    private Mock<IEncodingService>? _encodingService;
    private Mock<IEmployerAccountsApiService>? _apiService;
    private Mock<IEmployerFinanceApiService>? _financeApiService;
    private Mock<IMapper>? _mapper;
    private AccountWithBalanceViewModel? _expectedAccount;
    private AccountBalance? _expectedAccountBalance;

    [SetUp]
    public void Arrange()
    {
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger<AccountsOrchestrator>>();
        _encodingService = new Mock<IEncodingService>();
        _apiService = new Mock<IEmployerAccountsApiService>(); _financeApiService = new Mock<IEmployerFinanceApiService>();
        _orchestrator = new AccountsOrchestrator(_log.Object, _mapper.Object, _encodingService.Object, _apiService.Object, _financeApiService.Object);

        _expectedAccount = new AccountWithBalanceViewModel { AccountId = 124343 };
        _expectedAccountBalance = new AccountBalance { AccountId = _expectedAccount.AccountId };

        _apiService
            .Setup(x => x.GetAccounts(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedApiResponseViewModel<AccountWithBalanceViewModel?>
            {
                Data = new List<AccountWithBalanceViewModel> { _expectedAccount }
            });

        _financeApiService.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>(), CancellationToken.None))
            .ReturnsAsync(new List<AccountBalance?> { _expectedAccountBalance });

    }

    [Test]
    public async Task AndTheAccountHasALevyOverrideThenTheyShouldNotBeAllowedPaymentOnTheService()
    {
        //Arrange
        _expectedAccountBalance!.LevyOverride = false;

        //Act
        var response = await _orchestrator!.GetAllAccountsWithBalances(DateTime.Now.ToString(CultureInfo.InvariantCulture), 10, 1);

        //Assert
        Assert.That(response.Data.Data.First().IsAllowedPaymentOnService, Is.False);
    }

    [Test]
    public async Task AndTheAccountHasAnEoIAgreementThenTheyShouldBeAllowedPaymentOnTheService()
    {
        //Arrange
        _expectedAccount!.AccountAgreementType = AccountAgreementType.NonLevyExpressionOfInterest;

        //Act
        var response = await _orchestrator!.GetAllAccountsWithBalances(DateTime.Now.ToString(CultureInfo.InvariantCulture), 10, 1);

        //Assert
        Assert.That(response.Data.Data.First().IsAllowedPaymentOnService, Is.True);
    }

    [Test]
    public async Task AndTheEmployerTypeIsLevyThenTheyShouldBeAllowedPaymentOnTheService()
    {
        //Arrange
        _expectedAccount!.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy;

        //Act
        var response = await _orchestrator!.GetAllAccountsWithBalances(DateTime.Now.ToString(CultureInfo.InvariantCulture), 10, 1);

        //Assert
        Assert.That(response.Data.Data.First().IsAllowedPaymentOnService, Is.True);
    }

    [Test]
    public void AndTheEmployerTypeIsNoneLevyThenTheyShouldNotBeAllowedPaymentOnTheService()
    {
        //Arrange
        _expectedAccount!.ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy;

        //Act
        var response = _orchestrator!.GetAllAccountsWithBalances(DateTime.Now.ToString(CultureInfo.InvariantCulture), 10, 1);

        //Assert
        Assert.That(response.Result.Data.Data.First().IsAllowedPaymentOnService, Is.False);
    }

    [Test]
    public void AndThereAreNoSignedAgreementsThenTheyShouldNotBeAllowedPaymentOnTheService()
    {
        //Arrange
        _expectedAccount!.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy;
        _expectedAccount.AccountAgreementType = AccountAgreementType.Unknown;

        //Act
        var response = _orchestrator!.GetAllAccountsWithBalances(DateTime.Now.ToString(CultureInfo.InvariantCulture), 10, 1);

        //Assert
        Assert.That(response.Result.Data.Data.First().IsAllowedPaymentOnService, Is.False);
    }
}