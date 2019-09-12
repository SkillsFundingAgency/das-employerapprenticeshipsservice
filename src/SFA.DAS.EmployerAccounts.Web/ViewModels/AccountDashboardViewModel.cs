using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class AccountDashboardViewModel
    {
        public EmployerAccounts.Models.Account.Account Account { get; set; }
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
        public EAS.Portal.Client.Types.Account AccountViewModel { get; set; }
        public Guid? RecentlyAddedReservationId { get; set; }
        public Reservation ReservedFundingToShow => AccountViewModel?.Organisations?.SelectMany(org => org.Reservations).FirstOrDefault(rf => rf.Id == RecentlyAddedReservationId) ?? AccountViewModel?.Organisations?.SelectMany(org => org.Reservations)?.LastOrDefault();
        public string ReservedFundingOrgName => AccountViewModel?.Organisations?.Where(org => org.Reservations.Contains(ReservedFundingToShow)).Select(org => org.Name).FirstOrDefault();
        public bool ShowReservations => AccountViewModel?.Organisations?.FirstOrDefault()?.Reservations?.Count > 0;
        public bool HasSingleProvider => AccountViewModel?.Providers?.Count == 1;
        public bool HasMultipleProviders => AccountViewModel?.Providers?.Count > 1;
        // already returned in Account.ApprenticeshipEmployerType, but we want to transition to calling the api, rather than going direct to the db
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public AgreementInfoViewModel AgreementInfo { get; set; }
        
    }
}