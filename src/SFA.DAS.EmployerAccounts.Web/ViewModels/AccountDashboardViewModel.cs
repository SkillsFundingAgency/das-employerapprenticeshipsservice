using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class AccountDashboardViewModel
{
    public Account Account { get; set; }
    public string EmployerAccountType { get; set; }
    public string HashedUserId { get; set; }
    public int OrganisationCount { get; set; }
    public int PayeSchemeCount { get; set; }
    public int RequiresAgreementSigning { get; set; }
    public bool ShowAcademicYearBanner { get; set; }
    public bool ShowWizard { get; set; }
    public ICollection<AccountTask> Tasks { get; set; }
    public int TeamMemberCount { get; set; }
    public int TeamMembersInvited { get; set; }
    public string UserFirstName { get; set; }
    public Role UserRole { get; set; }
    public int SignedAgreementCount { get; set; }
    public List<PendingAgreementsViewModel> PendingAgreements { get; set; }
    public bool HasPayeScheme => PayeSchemeCount > 0;
    public AgreementInfoViewModel AgreementInfo { get; set; }
    public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
    public CallToActionViewModel CallToActionViewModel { get; set; }
    public bool HideTasksBar { get; set; }
    public DateTime? TermAndConditionsAcceptedOn { get; set; }
    public DateTime? LastTermsAndConditionsUpdate { get; set; }
    public bool ShowTermsAndConditionBanner => LastTermsAndConditionsUpdate.HasValue && (!TermAndConditionsAcceptedOn.HasValue || (TermAndConditionsAcceptedOn.Value < LastTermsAndConditionsUpdate.Value));

    public string SingleAccountLegalEntityId { get; set; }
    public string HashedAccountId { get; set; }
}