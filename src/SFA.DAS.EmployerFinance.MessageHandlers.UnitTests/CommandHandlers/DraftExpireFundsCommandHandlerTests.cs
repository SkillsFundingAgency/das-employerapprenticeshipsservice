using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.CommandHandlers
{
    [Parallelizable]
    public class DraftExpireFundsCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_Creates_Draft_Command_For_Each_Account(
            DraftExpireFundsCommand command,
            DateTime expectedDate,
            DateTime expectedRunDate,
            Models.Account.Account accountOne,
            Models.Account.Account accountTwo,
            [Frozen] Mock<ICurrentDateTime> currentDateTime,
            [Frozen] Mock<IEmployerAccountRepository> repository,
            Mock<IMessageHandlerContext> context,
            DraftExpireFundsCommandHandler handler
        )
        {
            currentDateTime.Setup(x => x.Now).Returns(expectedRunDate);
            command.DateTo = expectedDate;
            var accounts = new List<Models.Account.Account> {accountOne, accountTwo};
            repository.Setup(x => x.GetAll()).ReturnsAsync(accounts);

            await handler.Handle(command, context.Object);

            context.Verify(x =>
                    x.Send(
                        It.Is<DraftExpireAccountFundsCommand>(c =>
                            c.AccountId.Equals(accountOne.Id) && c.DateTo.Equals(command.DateTo)),
                        It.Is<SendOptions>(o => o.RequiredImmediateDispatch() && o.IsRoutingToThisEndpoint())),
                Times.Once());
            context.Verify(x =>
                    x.Send(
                        It.Is<DraftExpireAccountFundsCommand>(c =>
                            c.AccountId.Equals(accountTwo.Id) && c.DateTo.Equals(command.DateTo)),
                        It.Is<SendOptions>(o => o.RequiredImmediateDispatch()
                                                && o.IsRoutingToThisEndpoint())),
                Times.Once());
        }
    }
}