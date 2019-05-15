using SFA.DAS.EAS.Portal.Application.Commands.Cohort;
using System;

namespace SFA.DAS.EAS.Portal.Worker.UnitTests.Builders
{
    public class CohortApprovalRequestedCommandBuilder
    {
        private long _accountId;
        private long _providerId;
        private long _commitmentId;

        public CohortApprovalRequestedCommandBuilder()
        {
            var random = new Random();
            _accountId = random.Next(100, 999);
            _providerId = random.Next(100, 999);
            _commitmentId = random.Next(100, 999);
        }

        public CohortApprovalRequestedCommand Build()
        {
            return new CohortApprovalRequestedCommand(_accountId, _providerId, _commitmentId);
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
