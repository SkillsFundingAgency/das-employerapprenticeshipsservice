using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain;
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
        public async Task<ActionResult> Index(string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var model = await _orchestrator.Get(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

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
        public async Task<ActionResult> AddOrganisation(string hashedAccountId)
        {
            var response = await _orchestrator.GetAddLegalEntityViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Agreements/Add")]
        public async Task<ActionResult> AddOrganisation(string hashedAccountId, OrganisationType orgType, string companiesHouseNumber, string publicBodyName, string charityRegNo)
        {
            var searchTerm = "";
            switch (orgType)
            {
                case OrganisationType.Charities:
                    searchTerm = charityRegNo;
                    break;
                case OrganisationType.CompaniesHouse:
                    searchTerm = companiesHouseNumber;
                    break;
                case OrganisationType.PublicBodies:
                    searchTerm = publicBodyName;
                    break;
                case OrganisationType.Other:
                    searchTerm = String.Empty;
                    break;
                default:
                    throw new NotImplementedException("Org Type Not Implemented");
            }

            var response = await _orchestrator.FindLegalEntity(hashedAccountId, orgType, searchTerm, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Status == HttpStatusCode.OK)
            {
                return View("FindLegalEntity", response);
            }

            var errorResponse = new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel { HashedAccountId = hashedAccountId },
                Status = HttpStatusCode.OK,
            };

            if (response.Status == HttpStatusCode.NotFound)
            {
                switch (orgType)
                {
                    case OrganisationType.CompaniesHouse:
                        TempData["companyError"] = "Company not found";
                        break;
                    case OrganisationType.Charities:
                        TempData["charityError"] = "Charity not found";
                        break;
                    case OrganisationType.PublicBodies:
                        TempData["publicBodyError"] = "Public sector body not found";
                        break;
                    case OrganisationType.Other:
                        break;
                }              
            }

            if (response.Status == HttpStatusCode.Conflict)
            {
                TempData["companyError"] = "Enter a company that isn't already registered";
            }


            if (orgType == OrganisationType.Charities)
            {
                var charityResult = (FindCharityViewModel) response.Data;
                if (charityResult.IsRemovedError)
                {
                    TempData["charityError"] = "Charity is removed";
                }
            }

            if (orgType == OrganisationType.PublicBodies)
            {
                var pbresult = (FindPublicBodyViewModel) response.Data;
                if (pbresult.Results.Data.Count==1)
                {
                    return View("FindLegalEntity", response);
                }
                else
                {
                    return View("SelectPublicBody", response);
                }
            }

            return View("AddOrganisation", errorResponse);

        }


		[HttpGet]
		[Route("Agreements/{agreementid}/View")]
        public async Task<ActionResult> View(string agreementid, string hashedAccountId, FlashMessageViewModel flashMessage)
        {
            var agreement = await _orchestrator.GetById(agreementid, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));


            return View(agreement);
        }
        
        [HttpPost]
        [Route("Agreements/{agreementid}/Sign")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Sign(string agreementid, string hashedAccountId, string understood, string legalEntityName)
        {
            if (understood == nameof(understood))
            {
                var response = await _orchestrator.SignAgreement(agreementid, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), DateTime.Now);

                if (response.Status == HttpStatusCode.OK)
                {
                    TempData["agreementSigned"] = legalEntityName;

                    return RedirectToAction("Index", new { hashedAccountId });
                }

                return View("DeadView", response);
            }

            TempData["notunderstood"] = true;
           
            return RedirectToAction("View", new { agreementId = agreementid, hashedAccountId });
        }
        

        [HttpPost]
		[ValidateAntiForgeryToken]
        [Route("Agreements/ViewAgreement")]
        public async Task<ActionResult> ViewEntityAgreement(string hashedAccountId, string name, string code, string address, 
            DateTime incorporated)
        {
            var response = await _orchestrator.Create(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), name, code, address, incorporated);

            return View(response);
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
        [Route("Agreements/CreateAgreement")]
        public async Task<ActionResult> CreateLegalEntity(
            string hashedAccountId, string name, string code, string address, DateTime? incorporated, 
            bool? userIsAuthorisedToSign, string submit, string legalEntityStatus, OrganisationType legalEntitySource)
        {
            var request = new CreateNewLegalEntity
            {
                HashedAccountId = hashedAccountId,
                Name = name,
                Code = code,
                Address = address,
                IncorporatedDate = incorporated,
                UserIsAuthorisedToSign = userIsAuthorisedToSign ?? false,
                SignedAgreement = submit.Equals("Sign", StringComparison.CurrentCultureIgnoreCase),
                SignedDate = DateTime.Now,
                ExternalUserId = OwinWrapper.GetClaimValue(@"sub"),
                LegalEntityStatus = legalEntityStatus,
                Source = (short)legalEntitySource
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

            return RedirectToAction("Index", new { hashedAccountId });
        }
    }
}