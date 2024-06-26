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
    protected Mock<IAccountApiClient> AccountApiClient;
    protected Mock<IDatetimeService> DatetimeService;
    protected Mock<ILogger<Services.AccountRepository>> Logger;
    protected Mock<IPayeSchemeObfuscator> PayeSchemeObfuscator;
    protected Mock<IPayRefHashingService> HashingService;
    protected Mock<IEncodingService> EncodingService;
    protected Mock<IEmployerAccountsApiService> EmployerAccountsApiService;

    protected IAccountRepository? Sut;

    [SetUp]
    public void Setup()
    {
        HashingService = new();
        PayeSchemeObfuscator = new();
        AccountApiClient = new();
        EmployerAccountsApiService = new();
        Logger = new();
        DatetimeService = new();
        EncodingService = new();
        
        Sut = new Services.AccountRepository(
            AccountApiClient.Object,
            PayeSchemeObfuscator.Object,
            DatetimeService.Object,
            Logger.Object,
            HashingService.Object,
            EncodingService.Object,
            EmployerAccountsApiService.Object
        );
    }
}