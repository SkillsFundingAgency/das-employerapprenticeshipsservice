using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.Data;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.AccountLevyStatus
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class An_UpdateAccountToLevyHandler_
    {
        private UpdateAccountToLevyHandler _sut;
        private long _accountId = 90125;
        private Mock<IEmployerAccountRepository> _accountRepository;

        [SetUp]
        public void Setup()
        {
            _accountRepository
                =
                new  Mock<IEmployerAccountRepository>();

            _sut
                =
                new UpdateAccountToLevyHandler(
                    _accountRepository.Object);
        }

        [Test]
        public async Task Updates_Account_To_Be_Levy()
        {
            _sut
                .Handle(
                    new UpdateAccountToLevy(_accountId));

            _accountRepository
                .Verify(
                    m => 
                        m.SetAccountAsLevy(
                            _accountId));
        }

        [Test]
        public async Task Propagates_Errors()
        {
            _accountRepository
                .Setup(
                    m =>
                        m.SetAccountAsLevy(
                            It.IsAny<long>()))
                .ThrowsAsync(new TestException());

            Assert
                .ThrowsAsync<TestException>(
                    () =>
                    _sut
                        .Handle(
                            new UpdateAccountToLevy(_accountId)));
        }
    }

    public class TestException : Exception
    {
    }
}