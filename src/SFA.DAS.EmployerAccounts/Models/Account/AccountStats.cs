namespace SFA.DAS.EmployerAccounts.Models.Account
{
    public class AccountStats
    {
        public long AccountId { get; set; }
        public int PayeSchemeCount { get; set; }
        public int OrganisationCount { get; set; }
        public int TeamMemberCount { get; set; }
        public int TeamMembersInvited { get; set; }
    }
}
