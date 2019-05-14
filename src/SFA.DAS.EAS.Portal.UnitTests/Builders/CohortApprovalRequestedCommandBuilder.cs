using SFA.DAS.EAS.Portal.Application.Commands.Cohort;
using System;

namespace SFA.DAS.EAS.Portal.UnitTests.Builders
{
    public class CohortApprovalRequestedCommandBuilder
    {
        private string _messageId;
        private long _accountId;
        private long _providerId;
        private long _commitmentId;

        public CohortApprovalRequestedCommandBuilder()
        {
            _messageId = Guid.NewGuid().ToString();
            var random = new Random();
            _accountId = random.Next(100, 999);
            _providerId = random.Next(100, 999);
            _commitmentId = random.Next(100, 999);
        }

        public CohortApprovalRequestedCommand Build()
        {
            return new CohortApprovalRequestedCommand(_messageId, _accountId, _providerId, _commitmentId);
        }

        public CohortApprovalRequestedCommandBuilder WithAccountId(long accountId)
        {
            _accountId = accountId;
            return this;
        }

        public CohortApprovalRequestedCommandBuilder WithProviderId(long providerId)
        {
            _providerId = providerId;
            return this;
        }

        public CohortApprovalRequestedCommandBuilder WithCommitmentId(long commitmentId)
        {
            _commitmentId = commitmentId;
            return this;
        }

        public static implicit operator CohortApprovalRequestedCommand(CohortApprovalRequestedCommandBuilder instance)
        {
            return instance.Build();
        }
    }
}
