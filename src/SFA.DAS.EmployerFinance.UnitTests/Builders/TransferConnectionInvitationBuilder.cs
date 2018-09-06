using Moq;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using System;

namespace SFA.DAS.EmployerFinance.UnitTests.Builders
{
    public class TransferConnectionInvitationBuilder
    {
        private readonly Mock<TransferConnectionInvitation> _transferConnectionInvitation = new Mock<TransferConnectionInvitation> { CallBase = true };

        public TransferConnectionInvitationBuilder WithCreatedDate(DateTime createdDate)
        {
            _transferConnectionInvitation.SetupProperty(i => i.CreatedDate, createdDate);

            return this;
        }

        public TransferConnectionInvitationBuilder WithId(int id)
        {
            _transferConnectionInvitation.SetupProperty(i => i.Id, id);

            return this;
        }

        public TransferConnectionInvitationBuilder WithReceiverAccount(EmployerFinance.Models.Account.Account receiverAccount)
        {
            _transferConnectionInvitation.SetupProperty(i => i.ReceiverAccount, receiverAccount);
            _transferConnectionInvitation.SetupProperty(i => i.ReceiverAccountId, receiverAccount.Id);

            return this;
        }

        public TransferConnectionInvitationBuilder WithSenderAccount(EmployerFinance.Models.Account.Account senderAccount)
        {
            _transferConnectionInvitation.SetupProperty(i => i.SenderAccount, senderAccount);
            _transferConnectionInvitation.SetupProperty(i => i.SenderAccountId, senderAccount.Id);

            return this;
        }

        public TransferConnectionInvitationBuilder WithStatus(TransferConnectionInvitationStatus status)
        {
            _transferConnectionInvitation.SetupProperty(i => i.Status, status);

            return this;
        }

        public TransferConnectionInvitation Build()
        {
            return _transferConnectionInvitation.Object;
        }
    }
}