using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

public abstract class WhenTestingAccountRepository
{
    protected Mock<IAccountApiClient> AccountApiClient = new();
    protected Mock<IDatetimeService> DatetimeService = new();
    protected Mock<ILogger<Services.AccountRepository>> Logger = new();
    protected Mock<IPayeSchemeObfuscator> PayeSchemeObsfuscator = new();
    protected Mock<IPayRefHashingService> HashingService = new();
    protected readonly Mock<IEncodingService> EncodingService = new();
    protected readonly Mock<IEmployerAccountsApiService> EmployerAccountsApiService = new();

    protected IAccountRepository? Sut;

    [SetUp]
    public void Setup()
    {
        Sut = new Services.AccountRepository(
            AccountApiClient.Object,
            PayeSchemeObsfuscator.Object,
            DatetimeService.Object,
            Logger.Object,
            HashingService.Object,
            EncodingService.Object,
            EmployerAccountsApiService.Object
        );
    }
}