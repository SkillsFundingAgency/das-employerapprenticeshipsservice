using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAgreementController : Controller
    {
        [HttpGet]
        [Route("agreements")]
        public ActionResult Index(string hashedAccountId, bool agreementSigned = false)
        {
            return Redirect(Url.LegacyEasAccountAction($"agreements?agreementSigned={agreementSigned}"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/details")]
        public ActionResult Details(string agreementId, string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"agreements/{agreementId}/details{paramString}"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/view")]
        public ActionResult View(string agreementId, string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"agreements/{agreementId}/view{paramString}"));
        }

        [HttpGet]
        [Route("agreements/unsigned/view")]
        public ActionResult ViewUnsignedAgreements(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("agreements/unsigned/view"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/about-your-agreement")]
        public ActionResult AboutYourAgreement(string agreementId, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction($"agreements/{agreementId}/about-your-agreement"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/sign-your-agreement")]
        public ActionResult SignAgreement(GetEmployerAgreementRequest request)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"agreements/{request.AgreementId}/sign-your-agreement{paramString}"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/next")]
        public ActionResult NextSteps(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("agreements/{agreementId}/next"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/agreement-pdf")]
        public ActionResult GetPdfAgreement(string agreementId, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction($"agreements/{agreementId}/agreement-pdf"));
        }

        [HttpGet]
        [Route("agreements/{agreementId}/signed-agreement-pdf")]
        public ActionResult GetSignedPdfAgreement(string agreementId, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("agreements/{agreementId}/signed-agreement-pdf"));
        }

        [HttpGet]
        [Route("agreements/remove")]
        public ActionResult GetOrganisationsToRemove(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("agreements/remove"));
        }

        [HttpGet]
        [Route("agreements/remove/{agreementId}")]
        public ActionResult ConfirmRemoveOrganisation(string agreementId, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction($"agreements/{agreementId}/next"));
        }
    }
}