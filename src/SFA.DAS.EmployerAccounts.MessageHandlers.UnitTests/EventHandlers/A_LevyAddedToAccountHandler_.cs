using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerFinance.Messages.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class A_LevyAddedToAccountHandler_
    {
        private LevyAddedToAccountHandler _handler;
        private long _accountId = 90210;
        private Mock<IMediator> _mediatr;

        [SetUp]
        public void Setup()
        {
            _mediatr = new Mock<IMediator>();

            _handler
                =
                new LevyAddedToAccountHandler(
                    _mediatr.Object);
        }

        [Test]
        public async Task Updates_Account_To_Levy()
        {
            await
                _handler
                    .Handle(
                        new LevyAddedToAccount
                        {
                            AccountId = _accountId,
                            Amount = decimal.One
                        },
                        Mock.Of<IMessageHandlerContext>()
                    );

            _mediatr
                .Verify(
                    m =>
                        m.SendAsync(
                            It.Is<UpdateAccountToLevy>(
                                cmd =>
                                    cmd.AccountId == _accountId)));
        }
    }
}