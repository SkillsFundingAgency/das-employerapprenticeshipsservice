using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.AccountLevyStatus
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class WhenISetTheAccountLevyStatus
    {
        private AccountLevyStatusCommandHandler _accountLevyStatusCommandHandler;
        private Mock<IEmployerAccountRepository> _accountRepository;
        private Mock<ILogger<AccountLevyStatusCommandHandler>> _logger;
        private Mock<IEventPublisher> _eventPublisher;

        [SetUp]
        public void Setup()
        {
            _accountRepository = new Mock<IEmployerAccountRepository>();
            _logger = new Mock<ILogger<AccountLevyStatusCommandHandler>>();
            _eventPublisher = new Mock<IEventPublisher>();

            _accountLevyStatusCommandHandler = new AccountLevyStatusCommandHandler(
                _accountRepository.Object,
                _logger.Object,
                _eventPublisher.Object);
        }

        [Test]
        public async Task ThenTheAccountLevyStatusIsChangedAndAnEventIsPublished()
        {
            var accountId = 12345678;

            _accountRepository.Setup(x => x.GetAccountById(accountId)).ReturnsAsync(
                new Account
                {
                    ApprenticeshipEmployerType = (byte) ApprenticeshipEmployerType.Unknown
                });

            var command = new AccountLevyStatusCommand
            {
                AccountId = accountId,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy
            };

            await _accountLevyStatusCommandHandler.Handle(command, CancellationToken.None);

            _accountRepository.Verify(x => x.SetAccountLevyStatus(accountId, ApprenticeshipEmployerType.Levy), Times.Once);

            _eventPublisher.Verify(x => x.Publish(It.Is<ApprenticeshipEmployerTypeChangeEvent>(c => 
                c.AccountId.Equals(accountId) && 
                c.ApprenticeshipEmployerType.Equals(ApprenticeshipEmployerType.Levy))), Times.Once);
        }

        [Test]
        public async Task ThenIfTheNewStatusMatchesTheCurrentStatusNoChangeIsMade()
        {
            var accountId = 12345678;

            _accountRepository.Setup(x => x.GetAccountById(accountId)).ReturnsAsync(
                new Account
                {
                    ApprenticeshipEmployerType = (byte)ApprenticeshipEmployerType.NonLevy
                });

            var command = new AccountLevyStatusCommand
            {
                AccountId = accountId,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy
            };

            await _accountLevyStatusCommandHandler.Handle(command, CancellationToken.None);

            _accountRepository.Verify(x => x.SetAccountLevyStatus(accountId, ApprenticeshipEmployerType.Levy), Times.Never);

            _eventPublisher.Verify(x => x.Publish(It.Is<ApprenticeshipEmployerTypeChangeEvent>(c =>
                c.AccountId.Equals(accountId) &&
                c.ApprenticeshipEmployerType.Equals(ApprenticeshipEmployerType.Levy))), Times.Never);
        }

        [Test]
        public async Task ThenIfTheNewStatusAttemptsToUpdateALevyAccountToNonLevyNoChangeIsMade()
        {
            var accountId = 12345678;

            _accountRepository.Setup(x => x.GetAccountById(accountId)).ReturnsAsync(
                new Account
                {
                    ApprenticeshipEmployerType = (byte)ApprenticeshipEmployerType.Levy
                });

            var command = new AccountLevyStatusCommand
            {
                AccountId = accountId,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy
            };

            await _accountLevyStatusCommandHandler.Handle(command, CancellationToken.None);

            _accountRepository.Verify(x => x.SetAccountLevyStatus(accountId, ApprenticeshipEmployerType.Levy), Times.Never);

            _eventPublisher.Verify(x => x.Publish(It.Is<ApprenticeshipEmployerTypeChangeEvent>(c =>
                c.AccountId.Equals(accountId) &&
                c.ApprenticeshipEmployerType.Equals(ApprenticeshipEmployerType.Levy))), Times.Never);
        }

        [Test]
        public async Task ThenIfTheUpdateAttemptsToChangeAnAccountToUnknownNoChangeIsMade()
        {
            var accountId = 12345678;

            _accountRepository.Setup(x => x.GetAccountById(accountId)).ReturnsAsync(
                new Account
                {
                    ApprenticeshipEmployerType = (byte)ApprenticeshipEmployerType.NonLevy
                });

            var command = new AccountLevyStatusCommand
            {
                AccountId = accountId,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Unknown
            };

            await _accountLevyStatusCommandHandler.Handle(command, CancellationToken.None);

            _accountRepository.Verify(x => x.SetAccountLevyStatus(accountId, ApprenticeshipEmployerType.Levy), Times.Never);

            _eventPublisher.Verify(x => x.Publish(It.Is<ApprenticeshipEmployerTypeChangeEvent>(c =>
                c.AccountId.Equals(accountId) &&
                c.ApprenticeshipEmployerType.Equals(ApprenticeshipEmployerType.Levy))), Times.Never);
        }
    }
}