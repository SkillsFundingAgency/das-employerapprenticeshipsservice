using SFA.DAS.CommitmentsV2.Messages.Events;
using System;

namespace SFA.DAS.EAS.Portal.Worker.UnitTests.Builders
{
    public class CohortApprovalRequestedByProviderBuilder
    {
        private long _accountId;
        private long _providerId;
        private long _commitmentId;

        public CohortApprovalRequestedByProviderBuilder()
        {
            var random = new Random();
            _accountId = random.Next(100, 999);
            _providerId = random.Next(100, 999);
            _commitmentId = random.Next(100, 999);
        }

        public CohortApprovalRequestedByProvider Build()
        {
            return new CohortApprovalRequestedByProvider
            {
                AccountId = _accountId,
                ProviderId = _providerId,
                CommitmentId = _commitmentId
            };
        }

        public CohortApprovalRequestedByProviderBuilder WithAccountId(long accountId)
        {
            _accountId = accountId;
            return this;
        }

        public CohortApprovalRequestedByProviderBuilder WithProviderId(long providerId)
        {
            _providerId = providerId;
            return this;
        }

        public CohortApprovalRequestedByProviderBuilder WithCommitmentId(long commitmentId)
        {
            _commitmentId = commitmentId;
            return this;
        }

        public static implicit operator CohortApprovalRequestedByProvider(CohortApprovalRequestedByProviderBuilder instance)
        {
            return instance.Build();
        }
    }
}
