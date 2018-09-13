using Microsoft.Azure;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/apprentices/manage")]
    public class EmployerManageApprenticesController : Controller
    {
        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult ListAll(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/details", Name = "OnProgrammeApprenticeshipDetails")]
        [Route("{hashedApprenticeshipId}/details/statuschange", Name = "ChangeStatusSelectOption")]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprenticeship")]
        public ActionResult Details(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/whentoapply", Name = "WhenToApplyChange")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult WhenToApplyChange(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/confirm", Name = "StatusChangeConfirmation")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult StatusChangeConfirmation(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/confirm")]
        [Route("{hashedApprenticeshipId}/changes/view", Name = "ViewChanges")]
        [Route("{hashedApprenticeshipId}/changes/review", Name = "ReviewChanges")]
        [Route("{hashedApprenticeshipId}/datalock/restart", Name = "RequestRestart")]
        [Route("{hashedApprenticeshipId}/datalock/changes", Name = "RequestChanges")]
        public ActionResult ConfirmChanges(string hashedAccountId, string hashedApprenticeshipId)
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
            var baseUrl = CloudConfigurationManager.GetSetting(ControllerConstants.CommitmentsBaseUrlKeyName).EndsWith("/")
                ? CloudConfigurationManager.GetSetting(ControllerConstants.CommitmentsBaseUrlKeyName)
                : CloudConfigurationManager.GetSetting(ControllerConstants.CommitmentsBaseUrlKeyName) + "/";

            var path = Request.Url.AbsolutePath;

            return RedirectPermanent($"{baseUrl}{path}");

        }
    }
}