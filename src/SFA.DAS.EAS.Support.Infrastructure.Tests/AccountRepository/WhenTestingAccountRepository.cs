using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    public abstract class WhenTestingAccountRepository
    {
        protected Mock<IAccountApiClient> AccountApiClient;
        protected Mock<IDatetimeService> DatetimeService;
        protected Mock<ILogger<Services.AccountRepository>> Logger;
        protected Mock<IPayeSchemeObfuscator> PayeSchemeObfuscator;
        protected Mock<IHashingService> HashingService;

        protected IAccountRepository _sut;

        [SetUp]
        public void Setup()
        {
            AccountApiClient = new Mock<IAccountApiClient>();
            DatetimeService = new Mock<IDatetimeService>();
            Logger = new Mock<ILogger<Services.AccountRepository>>();
            PayeSchemeObfuscator = new Mock<IPayeSchemeObfuscator>();
            HashingService = new Mock<IHashingService>();

            _sut = new Services.AccountRepository(
                AccountApiClient.Object,
                PayeSchemeObfuscator.Object,
                DatetimeService.Object,
                Logger.Object,
                HashingService.Object);
        }
    }
}