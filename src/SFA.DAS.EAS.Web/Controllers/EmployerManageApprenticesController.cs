using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Attributes;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{hashedAccountId}/apprentices/manage")]
    public class EmployerManageApprenticesController : Controller
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _easConfig;
        private readonly ILog _logger;

        public EmployerManageApprenticesController(EmployerApprenticeshipsServiceConfiguration easConfig, ILog logger) : base()
        {
            _easConfig = easConfig;
            _logger = logger;
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public ActionResult ListAll(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/details", Name = "OnProgrammeApprenticeshipDetails")]
        [Route("{hashedApprenticeshipId}/details/statuschange", Name = "ChangeStatusSelectOption")]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprenticeship")]
        [Deprecated]
        public ActionResult Details(string hashedAccountId, string hashedApprenticeshipId)
        {
            _logger.Info($"To track Apprentice V1 details UrlReferrer Request: {HttpContext.Request.UrlReferrer}");

            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/whentoapply", Name = "WhenToApplyChange")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public ActionResult WhenToApplyChange(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/confirm", Name = "StatusChangeConfirmation")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
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
        [Deprecated]
        public ActionResult ConfirmChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [Route("paymentorder", Name = "PaymentOrder")]
        [Deprecated]
        public ActionResult PaymentOrder(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        private RedirectResult RedirectPermanentCommitmentsUrl()
        {
            var baseUrl = _easConfig.EmployerCommitmentsBaseUrl.EndsWith("/")
                ? _easConfig.EmployerCommitmentsBaseUrl
                : _easConfig.EmployerCommitmentsBaseUrl + "/";

            var path = Request.Url.AbsolutePath;

            return RedirectPermanent($"{baseUrl}{path}");
        }
    }
}