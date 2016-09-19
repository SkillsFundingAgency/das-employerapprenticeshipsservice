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
    [Authorize]
    public class EmployerAgreementController : BaseController
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly EmployerAgreementOrchestrator _orchestrator;

        public EmployerAgreementController(IOwinWrapper owinWrapper, EmployerAgreementOrchestrator orchestrator)
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
            var model = await _orchestrator.Get(accountId, _owinWrapper.GetClaimValue(@"sub"));

            //DANGER: Injected flash messages override the orchestrator response
            if (flashMessage != null)
            {
                model.FlashMessage = flashMessage;
            }

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
            var agreement = await _orchestrator.GetById(agreementid, accountId, _owinWrapper.GetClaimValue(@"sub"));
            
            //DANGER: Injected flash messages override the orchestrator response
            if (flashMessage != null) agreement.FlashMessage = flashMessage;

            return View(agreement);
        }
        
        [HttpPost]
        public async Task<ActionResult> Sign(long agreementid, long accountId, string understood, string legalEntityName)
        {
            if (understood == nameof(understood))
            {
                var response = await _orchestrator.SignAgreement(agreementid, accountId, _owinWrapper.GetClaimValue(@"sub"), DateTime.Now);

                if (response.Status == HttpStatusCode.OK)
                {
                    var successMessage = new FlashMessageViewModel
                    {
                        Headline = "Agreement signed",
                        Message = $"You've signed the agreement for {legalEntityName}",
                        Severity = FlashMessageSeverityLevel.Success
                    };

                    return RedirectToAction("Index", new {accountId, flashMessage = successMessage });
                }

                return View("DeadView", response);
            }

            TempData["notunderstood"] = new object();
            var errorMessage = new FlashMessageViewModel
            {
                Message = "You must indicate that you have read and understood the terms",
                Severity = FlashMessageSeverityLevel.Danger
            };          
            return RedirectToAction("View", new { agreementId = agreementid, accountId, flashMessage = errorMessage });
        }
        
        [HttpPost]
        public async Task<ActionResult> FindLegalEntity(long accountId, string entityReferenceNumber)
        {
            var response = await _orchestrator.FindLegalEntity(accountId, entityReferenceNumber, _owinWrapper.GetClaimValue(@"sub"));

            if (response.Status == HttpStatusCode.OK)
            {
                return View("FindLegalEntity",response);
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
            var response = await _orchestrator.Create(accountId, _owinWrapper.GetClaimValue(@"sub"), name, code, address, incorporated);

            return View(response);
        }

        [HttpPost]
        public async Task<ActionResult> CreateLegalEntity(
            long accountId, string name, string code, string address, DateTime incorporated, 
            bool? userIsAuthorisedToSign, string submit)
        {
            var request = new CreateNewLegalEntity
            {
                AccountId = accountId,
                Name = name,
                Code = code,
                Address = address,
                IncorporatedDate = incorporated,
                UserIsAuthorisedToSign = userIsAuthorisedToSign ?? false,
                SignedAgreement = submit.Equals("Sign", StringComparison.CurrentCultureIgnoreCase),
                SignedDate = DateTime.Now,
                ExternalUserId = _owinWrapper.GetClaimValue(@"sub")
            };

            var response = await _orchestrator.CreateLegalEntity(request);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                response.Status = HttpStatusCode.OK; 

                TempData["userNotAuthorised"] = "true";

                return View("ViewEntityAgreement", response);
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