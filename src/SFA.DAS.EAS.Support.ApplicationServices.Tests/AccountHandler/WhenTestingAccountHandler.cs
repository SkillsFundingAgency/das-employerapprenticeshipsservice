using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.AccountHandler
{
    public abstract class WhenTestingAccountHandler
    {
        protected readonly string Id = "123";
        protected Mock<IAccountRepository> MockAccountRepository;
        protected ApplicationServices.Services.AccountHandler Unit;

        [SetUp]
        public void Setup()
        {
            MockAccountRepository = new Mock<IAccountRepository>();

            Unit = new ApplicationServices.Services.AccountHandler(MockAccountRepository.Object);
        }
    }
}