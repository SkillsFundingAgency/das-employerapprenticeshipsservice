using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.CommandHandlers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class SetAccountLevyStatusCommandHandlerTests
    {
        private SetAccountLevyStatusCommandHandler _handler;
        private long _accountId = 90210;
        private Mock<IMediator> _mediatr;

        [SetUp]
        public void Setup()
        {
            _mediatr = new Mock<IMediator>();
            _handler = new SetAccountLevyStatusCommandHandler(_mediatr.Object);
        }

        [Test]
        public async Task Handle_AccountLevyStatusEvent()
        {
            await _handler.Handle(new SetAccountLevyStatusCommand
            {
                AccountId = _accountId,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy
            },
                Mock.Of<IMessageHandlerContext>());

            _mediatr.Verify(m => m.SendAsync(It.Is<AccountLevyStatusCommand>(cmd => cmd.AccountId == _accountId && cmd.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy)));
        }
    }
}