using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class EmployerAgreementController : BaseController
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly IEmployerAgreementOrchestrator _orchestrator;

        public EmployerAgreementController(IOwinWrapper owinWrapper, IEmployerAgreementOrchestrator orchestrator)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));
            _owinWrapper = owinWrapper;
            _orchestrator = orchestrator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(long accountId, FlashMessageViewModel flashMessage)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var model = await _orchestrator.Get(accountId, userIdClaim);

            //DANGER: Injected flash messages override the orchestrator response
            if (flashMessage != null) model.FlashMessage = flashMessage;

            return View(model);
        }

        public ActionResult Add(long accountId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            return View(new AddLegalEntityViewModel {AccountId = accountId});
        }

        public async Task<ActionResult> View(long agreementid, long accountId, FlashMessageViewModel flashMessage)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var agreement = await _orchestrator.GetById(agreementid, accountId, userIdClaim);
            
            //DANGER: Injected flash messages override the orchestrator response
            if (flashMessage != null) agreement.FlashMessage = flashMessage;

            return View(agreement);
        }
        
        [HttpPost]
        public async Task<ActionResult> Sign(long agreementid, long accountId, string understood, string legalEntityName)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            if (understood == "understood")
            {
                var response = await _orchestrator.SignAgreement(agreementid, accountId, userIdClaim);

                if (response.Status == HttpStatusCode.OK)
                {
                    var successMessage = new FlashMessageViewModel()
                    {
                        Headline = "Agreement signed",
                        Message = $"You've signed the agreement for {legalEntityName}",
                        Severity = FlashMessageSeverityLevel.Success
                    };

                    return RedirectToAction("Index", new {accountId = accountId, flashMessage = successMessage });
                }

                return View("DeadView", response);
            }

            TempData["notunderstood"] = new object();
            var errorMessage = new FlashMessageViewModel()
            {
                Message = "You must indicate that you have read and understood the terms",
                Severity = FlashMessageSeverityLevel.Danger
            };          
            return RedirectToAction("View", new { agreementId = agreementid, accountId = accountId, flashMessage = errorMessage });
        }
        
        [HttpPost]
        public async Task<ActionResult> FindLegalEntity(long accountId, string entityReferenceNumber)
        {
            var response = await _orchestrator.FindLegalEntity(accountId, entityReferenceNumber);
            
            return View(response.Data);
        }

        [HttpPost]
        public async Task<ActionResult> ViewEntityAgreement(long accountId, string name, string code, string address, 
            DateTime incorporated)
        {
            var response = await _orchestrator.Create(accountId, name, code, address, incorporated);

            return View(response);
        }

        [HttpPost]
        public async Task<ActionResult> CreateLegalEntity(
            long accountId, string name, string code, string address, DateTime incorporated, 
            bool? userIsAuthorisedToSign, string submit)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            
            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                return RedirectToAction("Index", "Home");
            }

            var request = new CreateNewLegalEntity
            {
                AccountId = accountId,
                Name = name,
                Code = code,
                Address = address,
                IncorporatedDate = incorporated,
                UserIsAuthorisedToSign = userIsAuthorisedToSign ?? false,
                SignedAgreement = submit.Equals("Sign", StringComparison.CurrentCultureIgnoreCase),
                ExternalUserId = userIdClaim
            };

            var response = await _orchestrator.CreateLegalEntity(request);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                response.Status = HttpStatusCode.OK; // We will deal with the error here
                return View("ViewEntityAgreement", response); //Redirect back to same page to change issue
            }

            var builder = new StringBuilder();

            builder.AppendLine($"{name} has been added to the levy account");

            if (request.UserIsAuthorisedToSign && request.SignedAgreement)
            {
                builder.AppendLine($"Agreement for {name} has been signed)");
            }

            TempData["successMessage"] = builder.ToString();

            return RedirectToAction("Index", new { accountId });
        }
    }
}