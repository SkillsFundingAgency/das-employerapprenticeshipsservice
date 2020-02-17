using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using Reservation = SFA.DAS.EmployerAccounts.Models.Reservations.Reservation;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class AccountDashboardViewModel : IAccountIdentifier
    {
        public EmployerAccounts.Models.Account.Account Account { get; set; }
        public string EmployerAccountType { get; set; }
        public string HashedAccountId { get; set; }
        public string HashedUserId { get; set; }
        public int OrganisationCount { get; set; }
        public int PayeSchemeCount { get; set; }
        //public int RequiresAgreementSigning { get; set; }
        public bool ShowAcademicYearBanner { get; set; }
        public bool ShowWizard { get; set; }
        public ICollection<AccountTask> Tasks { get; set; }
        public int TeamMemberCount { get; set; }
        public int TeamMembersInvited { get; set; }
        public string UserFirstName { get; set; }
        public Role UserRole { get; set; }
        //public int SignedAgreementCount { get; set; }
        //public List<PendingAgreementsViewModel> PendingAgreements { get; set; }
        public bool HasPayeScheme => PayeSchemeCount > 0;
        public AgreementInfoViewModel AgreementInfo { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public CallToActionViewModel CallToActionViewModel {get; set;}        
    }
}