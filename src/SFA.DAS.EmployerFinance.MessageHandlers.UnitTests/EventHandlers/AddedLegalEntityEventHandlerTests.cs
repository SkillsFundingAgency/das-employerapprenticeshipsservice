using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.EmployerFinance.Commands.CreateAccountLegalEntity;
using SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    [Parallelizable]
    internal class AddedLegalEntityEventHandlerTests : FluentTest<AddedLegalEntityEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldSendCreateAccountUserCommand()
        {
            return RunAsync(f => f.Handler.Handle(f.Message, Mock.Of<IMessageHandlerContext>()),
                f => f.Mediator.Verify(x => x.SendAsync(It.Is<CreateAccountLegalEntityCommand>(p =>
                        p.AccountId == f.AccountId &&
                        p.LegalEntityId == f.LegalEntityId &&
                        p.Deleted == null &&
                        p.Id == f.AccountLegalEntityId
                    ))));
        }
    }

    internal class AddedLegalEntityEventHandlerTestsFixture
    {
        public string MessageId = "messageId";
        public AddedLegalEntityEvent Message;
        public long AccountId = 111923;
        public string UserName = "Testorina";
        public Guid UserRef = Guid.NewGuid();
        public string OrganisationName = "Test Organisation One";
        public long AgreementId = 22781184;
        public long LegalEntityId = 984087;
        public long AccountLegalEntityId = 102098;
        public string AccountLegalEntityPublicHashedId = "BHDYWL";
        public OrganisationType OrganisationType = OrganisationType.PensionsRegulator;
        public string OrganisationReferenceNumber = "REF ONE";
        public string OrganisationAddress = "1, TEST YARD, SOUTH SHIELDS";

        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IMediator> Mediator { get; set; }
        public AddedLegalEntityEventHandler Handler;

        public AddedLegalEntityEventHandlerTestsFixture()
        {

            Message = new AddedLegalEntityEvent
            {
                AccountId = AccountId,
                UserRef = UserRef,
                Created = Created,
                AccountLegalEntityId = AccountLegalEntityId,
                AccountLegalEntityPublicHashedId = AccountLegalEntityPublicHashedId,
                AgreementId = AgreementId,
                LegalEntityId = LegalEntityId,
                OrganisationAddress = OrganisationAddress,
                OrganisationName = OrganisationName,
                OrganisationReferenceNumber = OrganisationReferenceNumber,
                OrganisationType = OrganisationType,
                UserName = UserName
            };

            Mediator = new Mock<IMediator>();
            Handler = new AddedLegalEntityEventHandler(Mediator.Object);
        }
    }
}
