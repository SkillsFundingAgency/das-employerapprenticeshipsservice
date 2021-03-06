﻿using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{hashedAccountId}/apprentices/manage")]
    public class EmployerManageApprenticesController : Controller
    {
        private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;
        private readonly ILog _logger;

        public EmployerManageApprenticesController(EmployerAccountsConfiguration employerAccountsConfiguration, ILog logger)
        {
            _employerAccountsConfiguration = employerAccountsConfiguration;
            _logger = logger;
        }

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
            _logger.Info($"To track Apprentice V1 details UrlReferrer Request: {HttpContext.Request.UrlReferrer}");

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
            var baseUrl = _employerAccountsConfiguration.EmployerCommitmentsBaseUrl.EndsWith("/")
                ? _employerAccountsConfiguration.EmployerCommitmentsBaseUrl
                : _employerAccountsConfiguration.EmployerCommitmentsBaseUrl + "/";

            var path = Request.Url.AbsolutePath;

            return RedirectPermanent($"{baseUrl}{path}");
        }
    }
}