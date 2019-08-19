using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Commands.LegalEntitySignAgreement;
using SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    [Parallelizable]
    internal class SignedAgreementEventHandlerTests : FluentTest<SignedAgreementEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldSendLegalEntitySignAgreementCommand()
        {
            return RunAsync(f => f.Handler.Handle(f.Message, Mock.Of<IMessageHandlerContext>()),
                f => f.Mediator.Verify(x => x.SendAsync(It.Is<LegalEntitySignAgreementCommand>(p =>
                    p.AccountId == f.AccountId &&
                    p.LegalEntityId == f.LegalEntityId &&
                    p.SignedAgreementVersion == f.SignedAgreementVersion &&
                    p.SignedAgreementId == f.AgreementId
                ))));
        }
    }

    internal class SignedAgreementEventHandlerTestsFixture
    {
        public string MessageId = "messageId";
        public SignedAgreementEvent Message;
        public long AccountId = 111923;
        public string UserName = "Testorina";
        public Guid UserRef = Guid.NewGuid();
        public string OrganisationName = "Test Organisation One";
        public long AgreementId = 22781184;
        public long LegalEntityId = 984087;
        public bool CohortCreated = true;
        public AgreementType AgreementType = AgreementType.Levy;
        public int SignedAgreementVersion = 3;

        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IMediator> Mediator { get; set; }
        public SignedAgreementEventHandler Handler;

        public SignedAgreementEventHandlerTestsFixture()
        {

            Message = new SignedAgreementEvent
            {
                AccountId = AccountId,
                UserRef = UserRef,
                Created = Created,
                AgreementId = AgreementId,
                LegalEntityId = LegalEntityId,
                OrganisationName = OrganisationName,
                UserName = UserName,
                AgreementType = AgreementType,
                CohortCreated = CohortCreated,
                SignedAgreementVersion = SignedAgreementVersion
            };

            Mediator = new Mock<IMediator>();
            Handler = new SignedAgreementEventHandler(Mediator.Object);
        }
    }
}