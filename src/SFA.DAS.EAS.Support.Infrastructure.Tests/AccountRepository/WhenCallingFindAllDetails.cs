using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using ResourceList = SFA.DAS.EAS.Account.Api.Types.ResourceList;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingFindAllDetails : WhenTestingAccountRepository
{
    PagedApiResponseViewModel<AccountWithBalanceViewModel> _pagedApiResponseViewModel;
    List<AccountWithBalanceViewModel> _accountWithBalanceViewModels;

    [SetUp]
    public void InitialiseTest()
    {
        _accountWithBalanceViewModels = new List<AccountWithBalanceViewModel>
        {
            new AccountWithBalanceViewModel
            {
                AccountId = 123,
                AccountHashId = "ERERE",
                Balance = 1000m,
                Href = "http://tempuri.org/account/ERERE",
                AccountName = "Test Account",
                IsLevyPayer = true
            },
            new AccountWithBalanceViewModel
            {
                AccountId = 345,
                AccountHashId = "CNDFJ",
                Balance = 1000m,
                Href = "http://tempuri.org/account/CNDFJ",
                AccountName = "Test Account 2",
                IsLevyPayer = true
            }
        };

        _pagedApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
        {
            Data = _accountWithBalanceViewModels,
            Page = 1,
            TotalPages = 2
        };

        Setup();
    }


    [Test]
    public async Task ItShouldReturnAnEmptyListIfGetAccountsThrowsAnException()
    {
        AccountApiClient
            .Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), 10, null))
             .ReturnsAsync(_pagedApiResponseViewModel);

        var e = new Exception("Some exception message");
        AccountApiClient
            .Setup(x => x.GetAccount(It.IsAny<string>()))
            .ThrowsAsync(e);

        _sut = new Services.AccountRepository(
                         AccountApiClient.Object,
                         PayeSchemeObfuscator.Object,
                         DatetimeService.Object,
                         Logger.Object,
                         HashingService.Object);

        var actual = await _sut.FindAllDetails(10, 1);


        AccountApiClient.Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), null), Times.AtLeastOnce);
        AccountApiClient.Verify(x => x.GetAccount(It.IsAny<string>()), Times.AtLeastOnce);

        Logger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ));

        Assert.IsNotNull(actual);
        var list = actual.ToList();
        CollectionAssert.IsEmpty(list);
    }

    [Test]
    public async Task ItShouldReturnAnEmptyListIfGetAccountsThrowsAnHttpRequestException()
    {
        AccountApiClient
         .Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), 10, null))
          .ReturnsAsync(_pagedApiResponseViewModel);

        var e = new HttpRequestException("Some exception message");
        AccountApiClient
            .Setup(x => x.GetAccount(It.IsAny<string>()))
            .ThrowsAsync(e);

        _sut = new Services.AccountRepository(
                         AccountApiClient.Object,
                         PayeSchemeObfuscator.Object,
                         DatetimeService.Object,
                         Logger.Object,
                         HashingService.Object);

        var actual = await _sut.FindAllDetails(10, 1);

        AccountApiClient.Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), null), Times.AtLeastOnce);
        AccountApiClient.Verify(x => x.GetAccount(It.IsAny<string>()), Times.AtLeastOnce);
        Logger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        Assert.IsNotNull(actual);
        CollectionAssert.IsEmpty(actual.ToList());
    }

    [Test]
    public async Task ItShouldReturnTheEntireListOfAccounts()
    {
        AccountApiClient.Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), 10, null))
            .ReturnsAsync(_pagedApiResponseViewModel);

        AccountApiClient
            .Setup(x => x.GetAccount(It.IsAny<string>()))
            .ReturnsAsync(new AccountDetailViewModel
            {
                PayeSchemes = new ResourceList(new List<ResourceViewModel>())
            });

        _sut = new Services.AccountRepository(
                        AccountApiClient.Object,
                        PayeSchemeObfuscator.Object,
                        DatetimeService.Object,
                        Logger.Object,
                        HashingService.Object);

        var actual = await _sut.FindAllDetails(10, 1);

        AccountApiClient.Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), null), Times.Once);
        AccountApiClient.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Exactly(2));
        Assert.IsNotNull(actual);
        var list = actual.ToList();
        CollectionAssert.IsNotEmpty(list);
        Assert.That(list.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task ItShouldReturnTheEntireListOfAccountsWhenAccountHasPayeScheme()
    {
        //Arrange
        AccountApiClient.Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), 10, null))
           .ReturnsAsync(_pagedApiResponseViewModel);

        AccountApiClient
            .Setup(x => x.GetAccount(It.IsAny<string>()))
            .ReturnsAsync(new AccountDetailViewModel
            {
                PayeSchemes = new ResourceList(new[] { new ResourceViewModel { Id = "1", Href = "/api/payeschemes/test1" } })
            });

        AccountApiClient.Setup(x => x.GetResource<PayeSchemeModel>(It.IsAny<string>()))
            .ReturnsAsync(new PayeSchemeModel { Name = "Test", Ref = "123" });

        var obscuredPayePayeScheme = "123/123456";
        PayeSchemeObfuscator.Setup(x => x.ObscurePayeScheme(It.IsAny<string>()))
            .Returns(obscuredPayePayeScheme);

        _sut = new Services.AccountRepository(
                        AccountApiClient.Object,
                        PayeSchemeObfuscator.Object,
                        DatetimeService.Object,
                        Logger.Object,
                        HashingService.Object);

        //Act
        var actual = await _sut.FindAllDetails(10, 1);

        //Assert
        AccountApiClient.Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), null), Times.Once);
        AccountApiClient.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Exactly(2));
        Assert.IsNotNull(actual);
        var list = actual.ToList();
        CollectionAssert.IsNotEmpty(list);
        Assert.That(list.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task ItShouldThrowsAnExceptionWhenAccountHasNoResourceForPayeScheme()
    {
        //Arrange
        AccountApiClient.Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), 10, null))
           .ReturnsAsync(_pagedApiResponseViewModel);

        AccountApiClient
            .Setup(x => x.GetAccount(It.IsAny<string>()))
            .ReturnsAsync(new AccountDetailViewModel
            {
                PayeSchemes = new ResourceList(new[] { new ResourceViewModel { Id = "1", Href = "/api/payeschemes/test1" } })
            });

        var e = new Exception("Some exception message");
        AccountApiClient.Setup(x => x.GetResource<PayeSchemeModel>(It.IsAny<string>()))
             .ThrowsAsync(e);

        var obscuredPayePayeScheme = "123/123456";
        PayeSchemeObfuscator.Setup(x => x.ObscurePayeScheme(It.IsAny<string>()))
            .Returns(obscuredPayePayeScheme);

        _sut = new Services.AccountRepository(
                        AccountApiClient.Object,
                        PayeSchemeObfuscator.Object,
                        DatetimeService.Object,
                        Logger.Object,
                        HashingService.Object);

        //Act
        var actual = await _sut.FindAllDetails(10, 1);

        //Assert            
        AccountApiClient.Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), null), Times.Once);
        AccountApiClient.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Exactly(2));

        Assert.IsNotNull(actual);
        var list = actual.ToList();
        CollectionAssert.IsNotEmpty(list);
        Assert.That(list.Count(), Is.EqualTo(2));
    }
}