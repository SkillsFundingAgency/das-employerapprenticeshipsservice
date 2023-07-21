namespace SFA.DAS.EmployerAccounts.Web.RouteValues
{
    public static class RouteNames
    {
        public const string EmployerTeamIndex = "employer-team-index";
        public const string EmployerAccountPayBillTriage = "empoyer-account-pay-bill";
        public const string EmployerAccountPayBillTriagePost = "empoyer-account-pay-bill-post";
        public const string AddPayeShutter = "add-paye-shutter";
        public const string NewAccountSaveProgress = "new-account-save-progress";
        public const string PartialAccountSaveProgress = "partial-account-save-progess";

        public const string EmployerAccountGetApprenticeshipFundingInAccount = "employer-account-get-apprentice-funding-in-account";
        public const string EmployerAccountPostApprenticeshipFundingInAccount = "post-employer-account-get-apprentice-funding-in-account";
        public const string EmployerAccountGetApprenticeshipFunding = "employer-account-get-apprentice-funding";
        public const string EmployerAccountPostApprenticeshipFunding = "post-employer-account-get-apprentice-funding";

        public const string SearchPensionRegulatorAddOrganisation = "search-pension-regulator-add-org";
        public const string SearchPensionRegulatorCreateAccount = "search-pension-regulator-create-account";

        public const string SignOut = "SignOut";
        public const string EmployerAgreementIndex = "employer-agreement-index";
        public const string EmployerAccountCreate = "employer-account-create";
        public const string GetSignedPdfAgreement = "get-signed-pdf-agreement";
        public const string GetPdfAgreement = "get-pdf-agreement";
        public const string OrganisationConfirm = "organisation-confirm";
        public const string OrganisationGoToNextStep = "organisation-go-to-next-step";
        public const string AboutYourAgreement = "about-your-agreement";
        public const string EmployerAgreementSign = "sign";
        public const string AgreementView = "agreement-view";
        public const string ProcessOrganisationReview = "process-organisation-review";
        public const string OrganisationPostUpdateSelection = "organisation-post-update-selection";
        
        public const string EmployerAccountPaye = "paye-index";
        public const string PayeDetails = "paye-details";
        public const string PayePostRemove = "paye-post-remove";
        public const string EmployerAccountPayeGateway = "account-paye-gateway";
        public const string EmployerAccountPayeGatewayInform = "account-paye-gateway-inform";
        public const string PayePostNextSteps = "paye-post-next-steps";


        public const string AccountName = "account-name";
        public const string AccountNamePost = "account-name-post";
        public const string RenameAccount = "account-rename";
        public const string RenameAccountPost = "account-rename-post";
        public const string AccountNameConfirm = "account-name-confirm";
        public const string AccountNameConfirmPost = "account-name-confirm-post";
        public const string AccountNameSuccess = "account-name-success";
        public const string CreateAccountSuccess = "create-account-success";

        public const string PostConfirmRemoveOrganisation = "post-confirm-remove-organisation";
        public const string EmployerAgreementSignYourAgreement = "sign-your-agreement";
        public const string EmployerTeamReview = "employer-team-review";
        public const string EmployerTeamGetChangeRole = "employer-team-change-role";
        public const string EmployerTeamView = "employer-team-view";
        public const string EmployerTeamInvite = "employer-team-invite";
        public const string EmployerTeamInvitePost = "employer-team-invite-post";
        public const string EmployerTeamInviteNextPost = "employer-team-invite-next-post";
        public const string EmployerTeamCancelInvitation = "employer-team-cacel-invite";
        public const string EmployerTeamCancelInvitationPost = "employer-team-cacel-invite-post";
        public const string EmployerTeamResendInvite = "employer-team-resend-invite";
        public const string EmployerTeamChangeRolePost = "employer-team-change-role-post";
        public const string RemoveTeamMember = "remove-team-member";
        public const string ConfirmRemoveTeamMember = "cofirm-remove-team-member";

        public const string InvitationAcceptPost = "invitation-accept";

        // Dynamic Homepage Triage
        public const string CreateAdvert = "create-advert";
        public const string CreateAdvertPost = "create-advert-post";
        public const string TriageCourse = "triage-course";
        public const string TriageCoursePost = "tirage-course-post";
        public const string TriageCannotSetupWithoutChosenCourseAndProvider = "triage-cannot-setup-without-course-provider";
        public const string TriageChosenProvider = "triage-chosen-provider";
        public const string TriageChosenProviderPost = "triage-chosen-provider-post";
        public const string TriageCannotSetupWithoutChosenProvider = "triage-cannot-setup-without-provider";
        public const string TriageWhenWillApprenticeshipStart = "triage-when-will-apprenticeship-start";
        public const string TriageWhenWillApprenticeshipStartPost = "triage-when-will-apprenticeship-start-post";
        public const string TriageCannotSetupWithoutStartDate = "triage-cannot-setup-without-start-date";
        public const string TriageCannotSetupWithoutApproximateStartDate = "triage-cannot-setup-without-approx-start-date";
        public const string TriageWhenApprenticeshipForExistingEmployee = "triage-existing-employee";
        public const string TriageWhenApprenticeshipForExistingEmployeePost = "triage-existing-employee-post";
        public const string NewEmpoyerAccountTaskList = "new-account-task-list";
        public const string ContinueNewEmployerAccountTaskList = "continue-account-task-list";

        public const string OrganisationAndPayeAddedSuccess = "org-and-paye-added-success";
    }
}
