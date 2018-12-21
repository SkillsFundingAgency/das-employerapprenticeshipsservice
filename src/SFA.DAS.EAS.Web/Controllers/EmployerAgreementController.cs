using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.ViewModels;
using System;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAgreementController : BaseController
    {
       
        public EmployerAgreementController(IAuthenticationService owinWrapper,
            IAuthorizationService authorization,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
        }

        [HttpGet]
        [Route("agreements")]
        public ActionResult Index()
        {
            return Redirect(Url.EmployerAccountsAction("agreements"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/details")]
        public ActionResult Details(string agreementId)
        {

            return
                Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/details"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/view")]
        public new ActionResult View(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/view"));
        }

        [HttpGet]
        [Route("agreements/unsigned/view")]
        public ActionResult ViewUnsignedAgreements()
        {
            return Redirect(Url.EmployerAccountsAction("agreements/unsigned/view"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public ActionResult AboutYourAgreement(string agreementid)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementid}/about-your-agreement"));

        }

        [HttpGet]
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public ActionResult SignAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/sign-your-agreement"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/next")]
        public ActionResult NextSteps(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/next"));
        }


        [HttpGet]
        [Route("agreements/{agreementId}/agreement-pdf")]
        public ActionResult GetPdfAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/agreement-pdf"));

        }

        [HttpGet]
        [Route("agreements/{agreementId}/signed-agreement-pdf")]
        public ActionResult GetSignedPdfAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/signed-agreement-pdf"));
        }

        [HttpGet]
        [Route("agreements/remove")]
        public ActionResult GetOrganisationsToRemove()
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/remove"));
        }

        [HttpGet]
        [Route("agreements/remove/{agreementId}")]
        public ActionResult ConfirmRemoveOrganisation(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/remove/{agreementId}"));
        }

        [HttpPost]
        [Route("agreements/remove/{agreementId}")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveOrganisation( string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/remove/{agreementId}"));
        }

    }
}