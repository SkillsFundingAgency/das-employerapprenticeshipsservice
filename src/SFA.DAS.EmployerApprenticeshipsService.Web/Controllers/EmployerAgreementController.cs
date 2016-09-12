using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

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

        [HttpGet]
        public ActionResult Add(long accountId)
        {
            var response = new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel {AccountId = accountId},
                Status = HttpStatusCode.OK
            };

            return View(response);
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
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var response = await _orchestrator.FindLegalEntity(accountId, entityReferenceNumber, userIdClaim);

            if (response.Status == HttpStatusCode.OK)
            {
                return View(response.Data);
            }

              var errorResponse = new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel { AccountId = accountId },
                Status = HttpStatusCode.OK,
            };

            if (response.Status == HttpStatusCode.NotFound)
            {
                TempData["companyNumberError"] = "No company found. Please try again";
            }

            if (response.Status == HttpStatusCode.Conflict)
            {
                TempData["companyNumberError"] = "Enter a company that isn't already registered";
            }

            return View("Add", errorResponse);
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

                TempData["userNotAuthorised"] = "true";

                return View("ViewEntityAgreement", response); //Redirect back to same page to change issue
            }

            if (request.UserIsAuthorisedToSign && request.SignedAgreement)
            {
                TempData["successHeader"] = $"{response.Data.EmployerAgreement.LegalEntityName} has been added";
                TempData["successMessage"] = "This account can now spend levy funds";
            }
            else
            {
                TempData["successHeader"] = $"{response.Data.EmployerAgreement.LegalEntityName} has been added";
                TempData["successMessage"] = "To spend the levy funs somebody needs to sign the agreement";
            }

            return RedirectToAction("Index", new { accountId });
        }
    }
}