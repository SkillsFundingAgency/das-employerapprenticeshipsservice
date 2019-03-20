using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.CommandHandlers
{
    [TestFixture]
    [Parallelizable]
    public class ExpireFundsCommandHandlerTests : FluentTest<ExpireFundsCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingCommand_ThenShouldSendCommands()
        {
            return RunAsync(
                f => f.Handle(),
                f => f.AccountIds.ForEach(i => f.Context.Verify(s => s.Send(
                    It.Is<ExpireAccountFundsCommand>(c => c.AccountId == i),
                    It.Is<SendOptions>(o => o.RequiredImmediateDispatch() && o.IsRoutingToThisEndpoint() && o.GetMessageId() == $"{f.Command.Month}-{f.Command.Year}-{i}")), Times.Once)));
        }
    }

    public class ExpireFundsCommandHandlerTestsFixture
    {
        public List<long> AccountIds { get; set; }
        public Mock<IMessageSession> MessageSession { get; set; }
        public ExpireFundsCommand Command { get; set; }
        public Mock<IMessageHandlerContext> Context { get; set; }
        public Mock<IEmployerAccountRepository> AccountRepository { get; set; }
        public IHandleMessages<ExpireFundsCommand> Handler { get; set; }

        public ExpireFundsCommandHandlerTestsFixture()
        {
            AccountIds = new List<long> { 1, 2, 3 };
            MessageSession = new Mock<IMessageSession>();
            Command = new ExpireFundsCommand { Month = 3, Year = 2019 };
            Context = new Mock<IMessageHandlerContext>();
            AccountRepository = new Mock<IEmployerAccountRepository>();
            Handler = new ExpireFundsCommandHandler(AccountRepository.Object);

            AccountRepository.Setup(r => r.GetAllAccounts()).ReturnsAsync(AccountIds.Select(i => new Account { Id = i }).ToList());
        }

        public Task Handle()
        {
            return Handler.Handle(Command, Context.Object);
        }
    }
}