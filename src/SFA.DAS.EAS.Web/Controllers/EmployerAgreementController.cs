using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [Route("accounts/{HashedAccountId}")]
    public class EmployerAgreementController : Controller
    {
        public EmployerApprenticeshipsServiceConfiguration Configuration { get; set; }
        public EmployerAgreementController(EmployerApprenticeshipsServiceConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [HttpGet]
        [Route("agreements")]
        public ActionResult Index()
        {
            return Redirect(Url.EmployerAccountsAction("agreements", Configuration));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/details")]
        public ActionResult Details(string agreementId)
        {

            return
                Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/details", Configuration));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/view")]
        public new ActionResult View(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/view", Configuration));
        }

        [HttpGet]
        [Route("agreements/unsigned/view")]
        public ActionResult ViewUnsignedAgreements()
        {
            return Redirect(Url.EmployerAccountsAction("agreements/unsigned/view", Configuration));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public ActionResult AboutYourAgreement(string agreementid)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementid}/about-your-agreement", Configuration));

        }

        [HttpGet]
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public ActionResult SignAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/sign-your-agreement", Configuration));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/next")]
        public ActionResult NextSteps(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/next", Configuration));
        }


        [HttpGet]
        [Route("agreements/{agreementId}/agreement-pdf")]
        public ActionResult GetPdfAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/agreement-pdf", Configuration));

        }

        [HttpGet]
        [Route("agreements/{agreementId}/signed-agreement-pdf")]
        public ActionResult GetSignedPdfAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/signed-agreement-pdf", Configuration));
        }

        [HttpGet]
        [Route("agreements/remove")]
        public ActionResult GetOrganisationsToRemove()
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/remove", Configuration));
        }

        [HttpGet]
        [Route("agreements/remove/{agreementId}")]
        public ActionResult ConfirmRemoveOrganisation(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/remove/{agreementId}", Configuration));
        }

        [HttpPost]
        [Route("agreements/remove/{agreementId}")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveOrganisation( string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/remove/{agreementId}", Configuration));
        }

    }
}