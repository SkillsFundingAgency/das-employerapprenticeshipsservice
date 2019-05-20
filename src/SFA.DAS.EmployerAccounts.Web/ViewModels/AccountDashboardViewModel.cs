using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Portal.Types;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class AccountDashboardViewModel
    {
        public Models.Account.Account Account { get; set; }
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
        public List<PendingAgreementsViewModel> PendingAgreements { get; set; }
        public bool ApprenticeshipAdded { get; set; }
        public bool ShowSearchBar { get; set; }
        public bool ShowMostActiveLinks { get; set; }
        public EAS.Portal.Types.Account AccountViewModel { get; set; }
        public Guid? RecentlyAddedReservationId { get; set; }
        public Reservation ReservedFundingToShow => AccountViewModel?.Organisations?.FirstOrDefault().Reservations?.FirstOrDefault(rf => rf.Id == RecentlyAddedReservationId) ?? AccountViewModel?.Organisations?.FirstOrDefault().Reservations?.FirstOrDefault();
        public bool ShowReservations => AccountViewModel?.Organisations?.FirstOrDefault().Reservations?.Count > 0;
    }
}   