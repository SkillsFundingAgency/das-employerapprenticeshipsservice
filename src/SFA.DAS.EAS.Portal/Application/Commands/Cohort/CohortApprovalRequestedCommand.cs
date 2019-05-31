namespace SFA.DAS.EAS.Portal.Application.Commands.Cohort
{
    public class CohortApprovalRequestedCommand : ICommand
    {        
        public long AccountId { get; private set; }
        public long ProviderId { get; private set; }
        public long CommitmentId { get; private set; }

        public CohortApprovalRequestedCommand(long accountId, long providerId, long commitmentId)
        {
            AccountId = accountId;
            ProviderId = providerId;
            CommitmentId = commitmentId;
        }
    }
}
