namespace SFA.DAS.EAS.Portal.Application.Commands.Cohort
{
    public class CohortApprovalRequestedCommand : BaseCommand
    {
        public override string MessageId { get; protected set; }
        public long AccountId { get; private set; }
        public long ProviderId { get; private set; }
        public long CommitmentId { get; private set; }

        public CohortApprovalRequestedCommand(string messageId, long accountId, long providerId, long commitmentId)
        {
            MessageId = messageId;
            AccountId = accountId;
            ProviderId = providerId;
            CommitmentId = commitmentId;
        }
    }
}
