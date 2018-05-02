using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class AccountDashboardViewModel
    {
        public string UserFirstName { get; set; }
        public Account Account { get; set; }
       
        public Role UserRole { get; set; }
        public string HashedUserId { get; set; }

        public string EmployerAccountType { get; set; }
        public bool ShowWizard { get; set; }

        public int PayeSchemeCount { get; set; }
        public int OrgainsationCount { get; set; }
        public int TeamMemberCount { get; set; }

        public bool ShowAcademicYearBanner { get; set; }
        public ICollection<AccountTask> Tasks { get; set; }

        public string HashedAccountId { get; set; }
    }
}