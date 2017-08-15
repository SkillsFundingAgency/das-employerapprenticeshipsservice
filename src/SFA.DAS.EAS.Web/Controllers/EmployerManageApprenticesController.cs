using System;
using System.Web.Mvc;

using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using Microsoft.Azure;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/apprentices/manage")]
    public class EmployerManageApprenticesController : BaseController
    {

        public EmployerManageApprenticesController(
            IOwinWrapper owinWrapper,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
                : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult ListAll(string hashedAccountId, ApprenticeshipFiltersViewModel filtersViewModel)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/details", Name = "OnProgrammeApprenticeshipDetails")]
        public ActionResult Details(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange", Name = "ChangeStatusSelectOption")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult ChangeStatus(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/whentoapply", Name = "WhenToApplyChange")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult WhenToApplyChange(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/confirm", Name = "StatusChangeConfirmation")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult StatusChangeConfirmation(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType, WhenToMakeChangeOptions whenToMakeChange, DateTime? dateOfChange)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprenticeship")]
        public ActionResult Edit(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/confirm")]
        public ActionResult ConfirmChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/view", Name = "ViewChanges")]
        public ActionResult ViewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/review", Name = "ReviewChanges")]
        public ActionResult ReviewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
             return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/datalock/restart", Name = "RequestRestart")]
        public ActionResult RequestRestart(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/datalock/changes", Name = "RequestChanges")]
        public ActionResult RequestChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("paymentorder", Name = "PaymentOrder")]
        public ActionResult PaymentOrder(string hashedAccountId)
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