using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using ResourceList = SFA.DAS.EAS.Account.Api.Types.ResourceList;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingFindAllDetails : WhenTestingAccountRepository
{
    private PagedApiResponseViewModel<AccountWithBalanceViewModel>? _pagedApiResponseViewModel;
    private List<AccountWithBalanceViewModel>? _accountWithBalanceViewModels;

    [SetUp]
    public void InitialiseTest()
    {
        _accountWithBalanceViewModels = new List<AccountWithBalanceViewModel>
        {
            new()
            {
                AccountId = 123,
                AccountHashId = "ERERE",
                Balance = 1000m,
                Href = "https://tempuri.org/account/ERERE",
                AccountName = "Test Account",
                IsLevyPayer = true
            },
            new()
            {
                AccountId = 345,
                AccountHashId = "CNDFJ",
                Balance = 1000m,
                Href = "https://tempuri.org/account/CNDFJ",
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
        EmployerAccountsApiService
            .Setup(x => x.GetAccounts(null, 10, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_pagedApiResponseViewModel)
            .Verifiable();
    }


    [Test]
    public async Task ItShouldReturnAnEmptyListIfGetAccountsThrowsAnException()
    {
        // Arrange
        var e = new Exception("Some exception message");
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(e)
            .Verifiable();

        // Act
        var actual = (await Sut.FindAllDetails(10, 1)).ToArray();

        // Assert
        EmployerAccountsApiService.Verify();
        actual.Should().NotBeNull();
        actual.Should().BeEmpty();
    }

    [Test]
    public async Task ItShouldReturnAnEmptyListIfGetAccountsThrowsAnHttpRequestException()
    {
        // Arrange
        var e = new HttpRequestException("Some exception message");
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(e);

        // Act
        var actual = await Sut.FindAllDetails(10, 1);

        // Assert
        EmployerAccountsApiService.Verify();
        actual.Should().NotBeNull();
        actual.Should().BeEmpty();
    }

    [Test]
    public async Task ItShouldReturnTheEntireListOfAccounts()
    {
        // Arrange
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountDetailViewModel
            {
                PayeSchemes = new ResourceList(new List<ResourceViewModel>())
            }).Verifiable(Times.Exactly(2));

        // Act
        var actual = await Sut.FindAllDetails(10, 1);

        // Assert
        EmployerAccountsApiService.Verify();
        actual.Should().NotBeNull();
        actual.Count().Should().Be(2);
    }

    [Test]
    public async Task ItShouldReturnTheEntireListOfAccountsWhenAccountHasPayeScheme()
    {
        //Arrange
        const string payeUri = "/api/payeschemes/scheme?ref=test1";
        const string payeRef = "123/AB23";
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountDetailViewModel
            {
                PayeSchemes = new ResourceList(new[]
                    { new ResourceViewModel { Id = "123%252fAB23", Href = payeUri } })
            });

        EmployerAccountsApiService.Setup(x => x.GetResource<PayeSchemeModel>(payeUri))
            .ReturnsAsync(new PayeSchemeModel { Name = "Test", Ref = payeRef });

        var obscuredPayePayeScheme = "1**/*B2*";
        PayeSchemeObfuscator.Setup(x => x.ObscurePayeScheme("123%252fAB23"))
            .Returns(obscuredPayePayeScheme);

        //Act
        var actual = await Sut.FindAllDetails(10, 1);

        //Assert
        EmployerAccountsApiService.Verify();
        actual.Should().NotBeNull();
        actual.Count().Should().Be(2);
        actual.First().PayeSchemes.First().Ref.Should().Be(payeRef);
    }

    [Test]
    public async Task ItShouldThrowsAnExceptionWhenAccountHasNoResourceForPayeScheme()
    {
        //Arrange
        const string payeUri = "/api/payeschemes/scheme?ref=test1";
        const string payeRef = "123/AB23";
        EmployerAccountsApiService
            .Setup(x => x.GetAccount(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountDetailViewModel
            {
                PayeSchemes = new ResourceList(new[]
                    { new ResourceViewModel { Id = "123%252fAB23", Href = payeUri } })
            }).Verifiable();

        var e = new Exception("Some exception message");
        EmployerAccountsApiService.Setup(x => x.GetResource<PayeSchemeModel>(payeUri))
            .ThrowsAsync(e)
            .Verifiable(Times.Exactly(2));

        var obscuredPayePayeScheme = "1**/*B2*";
        PayeSchemeObfuscator.Setup(x => x.ObscurePayeScheme("123%252fAB23"))
            .Returns(obscuredPayePayeScheme);

        //Act
        var actual = await Sut.FindAllDetails(10, 1);

        //Assert            
        EmployerAccountsApiService.Verify();
        actual.Should().NotBeNull();
        actual.Count().Should().Be(2);
    }
}