using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Commands.RemoveAccountLegalEntity;
using SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    [Parallelizable]
    internal class RemovedLegalEntityEventHandlerTests : FluentTest<RemovedLegalEntityEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldSendCreateAccountLegalEntityCommand()
        {
            return RunAsync(f => f.Handler.Handle(f.Message, Mock.Of<IMessageHandlerContext>()),
                f => f.Mediator.Verify(x => x.SendAsync(It.Is<RemoveAccountLegalEntityCommand>(p =>
                    p.Id == f.AccountLegalEntityId
                ))));
        }
    }

    internal class RemovedLegalEntityEventHandlerTestsFixture
    {
        public string MessageId = "messageId";
        public RemovedLegalEntityEvent Message;
        public long AccountId = 111923;
        public string UserName = "Testorina";
        public Guid UserRef = Guid.NewGuid();
        public string OrganisationName = "Test Organisation One";
        public long AgreementId = 22781184;
        public long LegalEntityId = 984087;
        public long AccountLegalEntityId = 102098;
        public bool AgreementSigned = true;

        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IMediator> Mediator { get; set; }
        public RemovedLegalEntityEventHandler Handler;

        public RemovedLegalEntityEventHandlerTestsFixture()
        {

            Message = new RemovedLegalEntityEvent()
            {
                AccountId = AccountId,
                UserRef = UserRef,
                Created = Created,
                AccountLegalEntityId = AccountLegalEntityId,
                AgreementId = AgreementId,
                LegalEntityId = LegalEntityId,
                OrganisationName = OrganisationName,
                UserName = UserName,
                AgreementSigned = AgreementSigned
            };

            Mediator = new Mock<IMediator>();
            Handler = new RemovedLegalEntityEventHandler(Mediator.Object);
        }
    }
}