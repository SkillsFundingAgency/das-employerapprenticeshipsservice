using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}/organisations")]
    public class OrganisationController : Controller
    {
        [HttpGet]
        [Route("nextStep")]
        public ActionResult OrganisationAddedNextSteps(
            string organisationName, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction($"organisations/nextStep?organisationName={organisationName}"));
        }

        [HttpGet]
        [Route("nextStepSearch")]
        public ActionResult OrganisationAddedNextStepsSearch(
            string organisationName, string hashedAccountId, string hashedAgreementId)
        {
            return Redirect(Url.LegacyEasAccountAction($"organisations/nextStepSearch?organisationName={organisationName}&hashedAgreementId={hashedAgreementId}"));
        }
    }
}