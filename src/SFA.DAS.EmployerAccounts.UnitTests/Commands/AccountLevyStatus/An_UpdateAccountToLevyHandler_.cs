using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.AccountLevyStatus
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class An_UpdateAccountToLevyHandler_
    {
        private UpdateAccountToLevyHandler _sut;
        private long _accountId = 90125;
        private Mock<IEmployerAccountRepository> _accountRepository;
        private Mock<ILog> _logger;

        private UpdateAccountToLevy _updateCommand;

        [SetUp]
        public void Setup()
        {
            _accountRepository
                =
                new Mock<IEmployerAccountRepository>();

            _logger
                =
                new Mock<ILog>();

            _sut
                =
                new UpdateAccountToLevyHandler(
                    _accountRepository.Object,
                    _logger.Object);

            _updateCommand = new UpdateAccountToLevy(_accountId);
        }

        [Test]
        public async Task Updates_Account_To_Be_Levy()
        {
            _sut
                .Handle(
                    _updateCommand);

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
                .Throws<TestException>();

            Assert
                .ThrowsAsync<TestException>(
                    () =>
                        _sut
                            .Handle(
                                _updateCommand));
        }

        [Test]
        public async Task Informs_That_Account_Is_To_Be_Updated()
        {
            _sut
                .Handle(
                    _updateCommand);

            _logger
                .Verify(
                    m =>
                        m.Info(_sut.UpdatedStartedMessage(_updateCommand)));
        }

        [Test]
        public async Task Informs_That_Account_Has_Been_Updated_On_Successful_Completion()
        {
            _sut
                .Handle(
                    _updateCommand);

            _logger
                .Verify(
                    m =>
                        m.Info(_sut.UpdateCompleteMessage(_updateCommand)));
        }

        [Test]
        public async Task Does_Not_Inform_That_Account_Has_Been_Updated_When_Error_Occurs()
        {
            _accountRepository
                .Setup(
                    m =>
                        m.SetAccountAsLevy(
                            It.IsAny<long>()))
                .Throws<TestException>();

            _sut
                .Handle(
                    _updateCommand);

            _logger
                .Verify(
                    m =>
                        m.Info(_sut.UpdateCompleteMessage(_updateCommand)),
                    Times.Never);
        }
    }

    public class TestException : Exception
    {
    }
}