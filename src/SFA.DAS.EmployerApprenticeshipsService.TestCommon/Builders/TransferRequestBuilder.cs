using System;
using Moq;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.TransferRequests;

namespace SFA.DAS.EAS.TestCommon.Builders
{
    public class TransferRequestBuilder
    {
        private readonly Mock<TransferRequest> _transferRequest = new Mock<TransferRequest> { CallBase = true };

        public TransferRequestBuilder WithCreatedDate(DateTime createdDate)
        {
            _transferRequest.SetupProperty(i => i.CreatedDate, createdDate);
            return this;
        }

        public TransferRequestBuilder WithCommitmentId(int commitmentId)
        {
            _transferRequest.SetupProperty(i => i.CommitmentId, commitmentId);
            return this;
        }

        public TransferRequestBuilder WithCommitmentHashedId(string commitmentHashedId)
        {
            _transferRequest.SetupProperty(i => i.CommitmentHashedId, commitmentHashedId);
            return this;
        }

        public TransferRequestBuilder WithReceiverAccount(Account receiverAccount)
        {
            _transferRequest.SetupProperty(i => i.ReceiverAccount, receiverAccount);
            return this;
        }

        public TransferRequestBuilder WithSenderAccount(Account senderAccount)
        {
            _transferRequest.SetupProperty(i => i.SenderAccount, senderAccount);
            return this;
        }

        public TransferRequestBuilder WithStatus(TransferRequestStatus status)
        {
            _transferRequest.SetupProperty(i => i.Status, status);
            return this;
        }

        public TransferRequestBuilder WithTransferCost(decimal transferCost)
        {
            _transferRequest.SetupProperty(i => i.TransferCost, transferCost);
            return this;
        }

        public TransferRequest Build()
        {
            return _transferRequest.Object;
        }
    }
}