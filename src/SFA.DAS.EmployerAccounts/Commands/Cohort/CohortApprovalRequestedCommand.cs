namespace SFA.DAS.EmployerAccounts.Commands.Cohort
{
    public class CohortApprovalRequestedCommand : ICommand
    {
        public long AccountId { get; private set; }

        public CohortApprovalRequestedCommand(long accountId)
        {
            AccountId = accountId;
        }
    }   
}