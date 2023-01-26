using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAgreementController : Microsoft.AspNetCore.Mvc.Controller
    { 
        [HttpGet]
        [Route("agreements")]
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return Redirect(Url.EmployerAccountsAction("agreements"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/details")]
        public Microsoft.AspNetCore.Mvc.ActionResult Details(string agreementId)
        {

            return
                Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/details"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/view")]
        public new Microsoft.AspNetCore.Mvc.ActionResult View(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/view"));
        }

        [HttpGet]
        [Route("agreements/unsigned/view")]
        public Microsoft.AspNetCore.Mvc.ActionResult ViewUnsignedAgreements()
        {
            return Redirect(Url.EmployerAccountsAction("agreements/unsigned/view"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public Microsoft.AspNetCore.Mvc.ActionResult AboutYourAgreement(string agreementid)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementid}/about-your-agreement"));

        }

        [HttpGet]
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public Microsoft.AspNetCore.Mvc.ActionResult SignAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/sign-your-agreement"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/next")]
        public Microsoft.AspNetCore.Mvc.ActionResult NextSteps(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/next"));
        }


        [HttpGet]
        [Route("agreements/{agreementId}/agreement-pdf")]
        public Microsoft.AspNetCore.Mvc.ActionResult GetPdfAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/agreement-pdf"));

        }

        [HttpGet]
        [Route("agreements/{agreementId}/signed-agreement-pdf")]
        public Microsoft.AspNetCore.Mvc.ActionResult GetSignedPdfAgreement(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/signed-agreement-pdf"));
        }

        [HttpGet]
        [Route("agreements/remove")]
        public Microsoft.AspNetCore.Mvc.ActionResult GetOrganisationsToRemove()
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/remove"));
        }

        [HttpGet]
        [Route("agreements/remove/{agreementId}")]
        public Microsoft.AspNetCore.Mvc.ActionResult ConfirmRemoveOrganisation(string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/remove/{agreementId}"));
        }

        [HttpPost]
        [Route("agreements/remove/{agreementId}")]
        [ValidateAntiForgeryToken]
        public Microsoft.AspNetCore.Mvc.ActionResult RemoveOrganisation( string agreementId)
        {
            return Redirect(Url.EmployerAccountsAction($"agreements/remove/{agreementId}"));
        }

    }
}