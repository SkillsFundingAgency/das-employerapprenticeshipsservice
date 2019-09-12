using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    public class AddedPayeSchemeEventHandlerTests : FluentTest<AddedPayeSchemeEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenThePayeSchemeWasAddedViaAorn_ThenShouldNotAddLevyToAccount()
        {
            return RunAsync(f =>
                {
                    f.Event.Aorn = "AORN";
                }, 
                f => f.Handle(), 
                f => f.Context.Verify(x => x.Send(It.IsAny<ImportAccountLevyDeclarationsCommand>(), It.IsAny<SendOptions>()), Times.Never()));
        }

        [Test]
        public Task Handle_WhenThePayeSchemeWasAddedViaGovernmentGateway_ThenLevyShouldBeAddedToAccount()
        {
            return RunAsync(f =>
                {
                    f.Event.Aorn = string.Empty;
                },
                f => f.Handle(),
                f => f.Context.Verify(x => x.Send(It.Is<ImportAccountLevyDeclarationsCommand>(y => y.AccountId == f.Event.AccountId && y.PayeRef == f.Event.PayeRef), It.IsAny<SendOptions>()), Times.Once()));
        }

        [Test]
        public Task Handle_WhenThePayeSchemeWasAddedViaAorn_ThenShouldCreateAccountPaye()
        {
            return RunAsync(f =>
                {
                    f.Event.Aorn = "AORN";
                },
                f => f.Handle(),
                f => f.Context.Verify(x => x.Send(It.Is<CreateAccountPayeCommand>(y => y.AccountId == f.Event.AccountId && y.EmpRef == f.Event.PayeRef && y.Aorn == f.Event.Aorn && y.Name == f.Event.SchemeName), It.IsAny<SendOptions>()), Times.Once()));
        }

        [Test]
        public Task Handle_WhenThePayeSchemeWasAddedViaGovernmentGateway_ThenShouldCreateAccountPaye()
        {
            return RunAsync(f =>
                {
                    f.Event.Aorn = string.Empty;
                },
                f => f.Handle(),
                f => f.Context.Verify(x => x.Send(It.Is<CreateAccountPayeCommand>(y => y.AccountId == f.Event.AccountId && y.EmpRef == f.Event.PayeRef && y.Aorn == f.Event.Aorn && y.Name == f.Event.SchemeName), It.IsAny<SendOptions>()), Times.Once()));
        }
    }

    public class AddedPayeSchemeEventHandlerTestsFixture
    {
        public IHandleMessages<AddedPayeSchemeEvent> Handler { get; set; }
        public Mock<IMessageHandlerContext> Context { get; set; }
        public AddedPayeSchemeEvent Event { get; set; }

        public AddedPayeSchemeEventHandlerTestsFixture()
        {
            Context = new Mock<IMessageHandlerContext>();

            Event = new AddedPayeSchemeEvent { AccountId = 12345, PayeRef = "ABC/1234565" };

            Handler = new AddedPayeSchemeEventHandler();
        }

        public Task Handle()
        {
            return Handler.Handle(Event, Context.Object);
        }
    }
}