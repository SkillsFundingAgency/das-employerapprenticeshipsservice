using System.Collections.Generic;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class AccountDashboardViewModel
    {
        public Account Account { get; set; }
        public string EmployerAccountType { get; set; }
        public string HashedAccountId { get; set; }
        public string HashedUserId { get; set; }
        public int OrgainsationCount { get; set; }
        public int PayeSchemeCount { get; set; }
        public int RequiresAgreementSigning { get; set; }
        public bool ShowAcademicYearBanner { get; set; }
        public bool ShowWizard { get; set; }
        public ICollection<AccountTask> Tasks { get; set; }
        public int TeamMemberCount { get; set; }
        public int TeamMembersInvited { get; set; }
        public string UserFirstName { get; set; }
        public Role UserRole { get; set; }
        public bool AgreementsToSign { get; set; }
        public int SignedAgreementCount { get; set; }
    }
}   