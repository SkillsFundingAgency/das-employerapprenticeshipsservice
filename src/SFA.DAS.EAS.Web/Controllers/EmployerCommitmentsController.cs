using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Enums;
using SFA.DAS.EAS.Web.ViewModels;
using Microsoft.Azure;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedaccountId}/apprentices")]
    public class EmployerCommitmentsController : BaseController
    {
        public EmployerCommitmentsController(IOwinWrapper owinWrapper,
            IFeatureToggle featureToggle, IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
        }

        [HttpGet]
        [Route("home", Name = "CommitmentsHome")]
        public ActionResult Index(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("cohorts")]
        public ActionResult YourCohorts(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("cohorts/draft")]
        public ActionResult Draft(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("cohorts/review")]
        public ActionResult ReadyForReview(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("cohorts/provider")]
        public ActionResult WithProvider(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("Inform")]
        public ActionResult Inform(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("legalEntity/create")]
        public ActionResult SelectLegalEntity(string hashedAccountId, string cohortRef = "")
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("provider/create")]
        public ActionResult SearchProvider(string hashedAccountId, string legalEntityCode, string cohortRef)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("confirmProvider/create")]
        public ActionResult ConfirmProvider(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("choosePath/create")]
        public ActionResult ChoosePath(string hashedAccountId, string legalEntityCode, string providerId, string cohortRef)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details")]
        public ActionResult Details(string hashedAccountId, string hashedCommitmentId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details/delete")]
        public ActionResult DeleteCohort(string hashedAccountId, string hashedCommitmentId)
        {
            return RedirectPermanentCommitmentsUrl();
        }


        [HttpGet]
        [Route("{legalEntityCode}/AgreementNotSigned")]
        public ActionResult AgreementNotSigned(LegalEntitySignedAgreementViewModel viewModel)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/apprenticeships/create")]
        public ActionResult CreateApprenticeshipEntry(string hashedAccountId, string hashedCommitmentId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/apprenticeships/{hashedApprenticeshipId}/edit")]
        public ActionResult EditApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/apprenticeships/{hashedApprenticeshipId}/view")]
        public ActionResult ViewApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/finished")]
        public ActionResult FinishedEditing(string hashedAccountId, string hashedCommitmentId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/CohortApproved")]
        public ActionResult Approved(string hashedAccountId, string hashedCommitmentId)
        {
            return RedirectPermanentCommitmentsUrl();
        }


        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/submit")]
        public ActionResult SubmitExistingCommitment(string hashedAccountId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("Submit")]
        public ActionResult SubmitNewCommitment(string hashedAccountId, string legalEntityCode, string legalEntityName, string legalEntityAddress, short legalEntitySource, string providerId, string providerName, string cohortRef, SaveStatus? saveStatus)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/NewCohortAcknowledgement")]
        public ActionResult AcknowledgementNew(string hashedAccountId, string hashedCommitmentId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public ActionResult AcknowledgementExisting(string hashedAccountId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Delete")]
        public ActionResult DeleteApprenticeshipConfirmation(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        private RedirectResult RedirectPermanentCommitmentsUrl()
        {
            var baseUrl = CloudConfigurationManager.GetSetting("EmployerCommitmentsBaseUrl").EndsWith("/")
                ? CloudConfigurationManager.GetSetting("EmployerCommitmentsBaseUrl")
                : CloudConfigurationManager.GetSetting("EmployerCommitmentsBaseUrl") + "/";

            var path = Request.Url.AbsolutePath;

            return RedirectPermanent($"{baseUrl}{path}");

        }
    }
}