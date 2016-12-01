using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAgreementController : BaseController
    {
        private readonly EmployerAgreementOrchestrator _orchestrator;

        public EmployerAgreementController(IOwinWrapper owinWrapper, EmployerAgreementOrchestrator orchestrator, 
            IFeatureToggle featureToggle, IUserWhiteList userWhiteList) 
            : base(owinWrapper, featureToggle, userWhiteList)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));
          
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("Agreements")]
        public async Task<ActionResult> Index(string HashedAccountId, FlashMessageViewModel flashMessage)
        {
            var model = await _orchestrator.Get(HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            if (TempData.ContainsKey("agreementSigned"))
            {
                model.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Agreement signed",
                    Message = $"You've signed the agreement for {TempData["agreementSigned"]}",
                    Severity = FlashMessageSeverityLevel.Success
                };
            }

            return View(model);
        }

        [HttpGet]
        [Route("Agreements/Add")]
        public async Task<ActionResult> Add(string HashedAccountId)
        {
            var response = await _orchestrator.GetAddLegalEntityViewModel(HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

		[HttpGet]
		[Route("Agreements/{agreementid}/View")]
        public async Task<ActionResult> View(string agreementid, string HashedAccountId, FlashMessageViewModel flashMessage)
        {
            var agreement = await _orchestrator.GetById(agreementid, HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));


            return View(agreement);
        }
        
        [HttpPost]
        [Route("Agreements/{agreementid}/Sign")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Sign(string agreementid, string HashedAccountId, string understood, string legalEntityName)
        {
            if (understood == nameof(understood))
            {
                var response = await _orchestrator.SignAgreement(agreementid, HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), DateTime.Now);

                if (response.Status == HttpStatusCode.OK)
                {
                    TempData["agreementSigned"] = legalEntityName;

                    return RedirectToAction("Index", new { HashedAccountId });
                }

                return View("DeadView", response);
            }

            TempData["notunderstood"] = true;
           
            return RedirectToAction("View", new { agreementId = agreementid, HashedAccountId });
        }
        
        [HttpPost]
		[ValidateAntiForgeryToken]
        [Route("Agreements/Add")]
        public async Task<ActionResult> FindLegalEntity(string HashedAccountId, string entityReferenceNumber)
        {
            var response = await _orchestrator.FindLegalEntity(HashedAccountId, entityReferenceNumber, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Status == HttpStatusCode.OK)
            {
                return View("FindLegalEntity",response);
            }

            var errorResponse = new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel { HashedAccountId = HashedAccountId },
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
		[ValidateAntiForgeryToken]
        [Route("Agreements/ViewAgreement")]
        public async Task<ActionResult> ViewEntityAgreement(string HashedAccountId, string name, string code, string address, 
            DateTime incorporated)
        {
            var response = await _orchestrator.Create(HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), name, code, address, incorporated);

            return View(response);
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
        [Route("Agreements/CreateAgreement")]
        public async Task<ActionResult> CreateLegalEntity(
            string HashedAccountId, string name, string code, string address, DateTime incorporated, 
            bool? userIsAuthorisedToSign, string submit)
        {
            var request = new CreateNewLegalEntity
            {
                HashedAccountId = HashedAccountId,
                Name = name,
                Code = code,
                Address = address,
                IncorporatedDate = incorporated,
                UserIsAuthorisedToSign = userIsAuthorisedToSign ?? false,
                SignedAgreement = submit.Equals("Sign", StringComparison.CurrentCultureIgnoreCase),
                SignedDate = DateTime.Now,
                ExternalUserId = OwinWrapper.GetClaimValue(@"sub")
            };

            var response = await _orchestrator.CreateLegalEntity(request);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                response.Status = HttpStatusCode.OK; 

                TempData["userNotAuthorised"] = "true";

                return View("ViewEntityAgreement", response);
            }

            TempData["extraCompanyAdded"] = "true";

            if (request.UserIsAuthorisedToSign && request.SignedAgreement)
            {
                TempData["successHeader"] = $"{response.Data.EmployerAgreement.LegalEntityName} has been added";
                TempData["successMessage"] = "This account can now spend levy funds.";
            }
            else
            {
                TempData["successHeader"] = $"{response.Data.EmployerAgreement.LegalEntityName} has been added";
                TempData["successMessage"] = "To spend the levy funds somebody needs to sign the agreement.";
            }

            return RedirectToAction("Index", new { HashedAccountId });
        }
    }
}