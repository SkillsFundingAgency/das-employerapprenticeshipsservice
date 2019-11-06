﻿using System.Configuration;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{hashedaccountId}/apprentices")]
    public class EmployerCommitmentsController : Controller
    {
        [HttpGet]
        [Route("home", Name = "CommitmentsHome")]
        [Route("cohorts/review")]
        [Route("cohorts/provider")]
        [Route("Inform")]
        [Route("confirmProvider/create")]
        public ActionResult Index(string hashedAccountId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("cohorts")]
        [Route("cohorts/draft")]
        public ActionResult YourCohorts(string hashedAccountId)
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
        [Route("choosePath/create")]
        public ActionResult ChoosePath(string hashedAccountId, string legalEntityCode, string providerId, string cohortRef)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details")]
        [Route("{hashedCommitmentId}/details/delete")]
        [Route("{hashedCommitmentId}/apprenticeships/create")]
        [Route("{hashedCommitmentId}/finished")]
        [Route("{hashedCommitmentId}/CohortApproved")]
        public ActionResult Details(string hashedAccountId, string hashedCommitmentId)
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
        [Route("{hashedCommitmentId}/apprenticeships/{hashedApprenticeshipId}/edit")]
        [Route("{hashedCommitmentId}/apprenticeships/{hashedApprenticeshipId}/view")]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Delete")]
        public ActionResult EditApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/submit")]
        public ActionResult SubmitExistingCommitment(string hashedAccountId, string hashedCommitmentId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("Submit")]
        public ActionResult SubmitNewCommitment(string hashedAccountId, string legalEntityCode, string legalEntityName, string legalEntityAddress, short legalEntitySource, string providerId, string providerName, string cohortRef)
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
        public ActionResult AcknowledgementExisting(string hashedAccountId, string hashedCommitmentId)
        {
            return RedirectPermanentCommitmentsUrl();
        }

        private RedirectResult RedirectPermanentCommitmentsUrl()
        {
            var baseUrl = ConfigurationManager.AppSettings[ControllerConstants.EmployerCommitmentsBaseUrlKeyName].EndsWith("/")
                ? ConfigurationManager.AppSettings[ControllerConstants.EmployerCommitmentsBaseUrlKeyName]
                : ConfigurationManager.AppSettings[ControllerConstants.EmployerCommitmentsBaseUrlKeyName] + "/";

            var path = Request.Url.AbsolutePath;

            return RedirectPermanent($"{baseUrl}{path}");
        }
    }
}