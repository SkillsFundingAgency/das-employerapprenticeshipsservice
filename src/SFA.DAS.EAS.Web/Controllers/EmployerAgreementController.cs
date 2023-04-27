using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[DasAuthorize]
[Route("accounts/{HashedAccountId}")]
public class EmployerAgreementController : Controller
{
    public IConfiguration Configuration { get; set; }
    public EmployerAgreementController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }

    [HttpGet]
    [Route("agreements")]
    public IActionResult Index()
    {
        return Redirect(Url.EmployerAccountsAction("agreements", Configuration));
    }

    [HttpGet]
    [Route("agreements/{agreementId}/details")]
    public IActionResult Details(string agreementId)
    {
        return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/details", Configuration));
    }

    [HttpGet]
    [Route("agreements/{agreementId}/view")]
    public new IActionResult View(string agreementId)
    {
        return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/view", Configuration));
    }

    [HttpGet]
    [Route("agreements/unsigned/view")]
    public IActionResult ViewUnsignedAgreements()
    {
        return Redirect(Url.EmployerAccountsAction("agreements/unsigned/view", Configuration));
    }

    [HttpGet]
    [Route("agreements/{agreementId}/about-your-agreement")]
    public IActionResult AboutYourAgreement(string agreementid)
    {
        return Redirect(Url.EmployerAccountsAction($"agreements/{agreementid}/about-your-agreement", Configuration));

    }

    [HttpGet]
    [Route("agreements/{agreementId}/sign-your-agreement")]
    public IActionResult SignAgreement(string agreementId)
    {
        return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/sign-your-agreement", Configuration));
    }

    [HttpGet]
    [Route("agreements/{agreementId}/next")]
    public IActionResult NextSteps(string agreementId)
    {
        return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/next", Configuration));
    }

    [HttpGet]
    [Route("agreements/{agreementId}/agreement-pdf")]
    public IActionResult GetPdfAgreement(string agreementId)
    {
        return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/agreement-pdf", Configuration));

    }

    [HttpGet]
    [Route("agreements/{agreementId}/signed-agreement-pdf")]
    public IActionResult GetSignedPdfAgreement(string agreementId)
    {
        return Redirect(Url.EmployerAccountsAction($"agreements/{agreementId}/signed-agreement-pdf", Configuration));
    }

    [HttpGet]
    [Route("agreements/remove")]
    public IActionResult GetOrganisationsToRemove()
    {
        return Redirect(Url.EmployerAccountsAction($"agreements/remove", Configuration));
    }

    [HttpGet]
    [Route("agreements/remove/{agreementId}")]
    public IActionResult ConfirmRemoveOrganisation(string agreementId)
    {
        return Redirect(Url.EmployerAccountsAction($"agreements/remove/{agreementId}", Configuration));
    }

    [HttpPost]
    [Route("agreements/remove/{agreementId}")]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveOrganisation(string agreementId)
    {
        return Redirect(Url.EmployerAccountsAction($"agreements/remove/{agreementId}", Configuration));
    }
}