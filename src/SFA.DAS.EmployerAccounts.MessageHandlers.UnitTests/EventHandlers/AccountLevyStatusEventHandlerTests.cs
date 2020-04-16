using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerFinance.Messages.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class AccountLevyStatusEventHandlerTests
    {
        private AccountLevyStatusEventHandler _handler;
        private long _accountId = 90210;
        private Mock<IMediator> _mediatr;

        [SetUp]
        public void Setup()
        {
            _mediatr = new Mock<IMediator>();
            _handler = new AccountLevyStatusEventHandler(_mediatr.Object);
        }

        [Test]
        public async Task Handle_AccountLevyStatusEvent()
        {
            await _handler.Handle(new AccountLevyStatusEvent
            {
                AccountId = _accountId,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy
            },
                Mock.Of<IMessageHandlerContext>());

            _mediatr.Verify(m => m.SendAsync(It.Is<AccountLevyStatusCommand>(cmd => cmd.AccountId == _accountId && cmd.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy)));
        }
    }
}