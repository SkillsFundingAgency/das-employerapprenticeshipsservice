using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class OrganisationController : BaseController
    {
        private readonly OrganisationOrchestrator _orchestrator;
        private readonly IMapper _mapper;

        public OrganisationController(
            IOwinWrapper owinWrapper, 
            OrganisationOrchestrator orchestrator,
            IFeatureToggle featureToggle, 
            IUserWhiteList userWhiteList,
            IMapper mapper) 
            :base(owinWrapper, featureToggle, userWhiteList)
        {
            _orchestrator = orchestrator;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("Organisation/Add")]
        public async Task<ActionResult> AddOrganisation(string hashedAccountId)
        {
            var response = await _orchestrator.GetAddLegalEntityViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Organisation/Add")]
        public async Task<ActionResult> AddOrganisation(AddLegalEntityViewModel model)
        {
            OrchestratorResponse<OrganisationDetailsViewModel> response;

            if (!model.OrganisationType.HasValue)
            {
                var orgTypeErrorResponse = new OrchestratorResponse<AddLegalEntityViewModel>
                {
                    Data = new AddLegalEntityViewModel()
                };
                orgTypeErrorResponse.Data.ErrorDictionary["orgType"] = "Select a type of organisation";
                orgTypeErrorResponse.Status = HttpStatusCode.BadRequest;
                orgTypeErrorResponse.FlashMessage = new FlashMessageViewModel
                {
                    Severity = FlashMessageSeverityLevel.Error,
                    ErrorMessages = orgTypeErrorResponse.Data.ErrorDictionary,
                    Headline = "Errors to fix",
                    Message = "Check the following details:"
                    
                };
                return View(orgTypeErrorResponse);
            }

            switch (model.OrganisationType)
            {
                case OrganisationType.Charities:
                    response = await FindCharity(model.CharityRegistrationNumber, model.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
                    break;
                case OrganisationType.CompaniesHouse:
                    response = await FindCompany(model.CompaniesHouseNumber, model.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

                    break;
                case OrganisationType.PublicBodies:
                    var searchResponse = await FindPublicSectorOrganisation(model.PublicBodyName, model.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
                    
                    if (searchResponse.Data.Results.Data.Count != 1 || searchResponse.Data.Results.Data.All(x => x.AddedToAccount))
                    {
                        return View("ViewPublicSectorOrganisationSearchResults", searchResponse);
                    }

                    response = new OrchestratorResponse<OrganisationDetailsViewModel>
                    {
                        Data = searchResponse.Data.Results.Data.FirstOrDefault(),
                        Status = searchResponse.Status
                    };
                    break;

                case OrganisationType.Other:
                    return RedirectToAction("AddOtherOrganisationDetails", "Organisation", new { model.HashedAccountId });
                    
                default:
                    throw new NotImplementedException("Org Type Not Implemented");
            }

            if (response.Status == HttpStatusCode.OK)
            {
                //Removes empty address entries (i.e. ' , , ')
                var address = (response.Data?.Address ?? string.Empty).Replace(",", string.Empty);

                if (string.IsNullOrWhiteSpace(address))
                {
                    var addressViewModel = _mapper.Map<AddOrganisationAddressModel>(response.Data);

                    var addressResponse = new OrchestratorResponse<AddOrganisationAddressModel>
                    {
                        Data = addressViewModel
                    };

                    return View("AddOrganisationAddress", addressResponse);
                }

                return View("ConfirmOrganisationDetails", response);
            }

            var errorResponse = new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = model,
                Status = HttpStatusCode.OK
            };
            errorResponse.Data.ErrorDictionary = response.Data.ErrorDictionary;

            errorResponse.FlashMessage = new FlashMessageViewModel
            {
                Severity = FlashMessageSeverityLevel.Error,
                ErrorMessages = errorResponse.Data.ErrorDictionary,
                Headline = "Errors to fix",
                Message = "Check the following details:"
            };

            return View("AddOrganisation", errorResponse);
        }

        [HttpGet]
        [Route("Organisation/UpdateAddress")]
        public ActionResult AddOrganisationAddress(AddOrganisationAddressModel request)
        {
            var response = new OrchestratorResponse<AddOrganisationAddressModel>
            {
                Data = request,
                Status = HttpStatusCode.OK
            };

            return View("AddOrganisationAddress", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Organisation/UpdateAddress")]
        public ActionResult UpdateOrganisationAddress(AddOrganisationAddressModel request)
        {
            var response = _orchestrator.AddOrganisationAddress(request);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                request.ErrorDictionary = response.Data.ErrorDictionary;

                var errorResponse = new OrchestratorResponse<AddOrganisationAddressModel>
                {
                    Data = request,
                    Status = HttpStatusCode.BadRequest,
                    Exception = response.Exception,
                    FlashMessage = response.FlashMessage
                };

                return View("AddOrganisationAddress", errorResponse);
            }

            return View("ConfirmOrganisationDetails", response);
        }

        private async Task<OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>> FindPublicSectorOrganisation(string publicSectorOrganisationName, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.FindPublicSectorOrganisation(publicSectorOrganisationName, hashedAccountId, userIdClaim);

            switch (response.Status)
            {
                case HttpStatusCode.NotFound:
                    TempData["publicBodyError"] = "No public organsiations were not found using your search term";
                    break;
            }

            return response;
        }

        private async Task<OrchestratorResponse<OrganisationDetailsViewModel>> FindCompany(string companiesHouseNumber, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.GetLimitedCompanyByRegistrationNumber(companiesHouseNumber, hashedAccountId, userIdClaim);
            return response;
        }

        private async Task<OrchestratorResponse<OrganisationDetailsViewModel>> FindCharity(string charityRegNo, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.GetCharityByRegistrationNumber(charityRegNo, hashedAccountId, userIdClaim);

            switch (response.Status)
            {
                case HttpStatusCode.NotFound:
                    TempData["charityError"] = "Charity not found";
                    break;
                case HttpStatusCode.BadRequest:
                    TempData["charityError"] = "Charity is removed";
                    break;
                case HttpStatusCode.Conflict:
                    TempData["charityError"] = "Charity is already added";
                    break;
            }
            
            return response;
        }

        [HttpGet]
        [Route("Organisation/Add/Other")]
        public ActionResult AddOtherOrganisationDetails(string hashedAccountId)
        {
            var response = _orchestrator.GetAddOtherOrganisationViewModel(hashedAccountId);
            return View(response);
        }

        [HttpPost]
        [Route("Organisation/Add/Other")]
        public async Task<ActionResult> AddOtherOrganisationDetails(OrganisationDetailsViewModel model)
        {
            var response = await _orchestrator.ValidateLegalEntityName(model);

            if (response.Status == HttpStatusCode.OK)
            {
                var addressResponse = _orchestrator.CreateAddOrganisationAddressViewModelFromOrganisationDetails(model);
                return View("AddOrganisationAddress", addressResponse);
            }

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Organisation/Confirm")]
        public async Task<ActionResult> Confirm(
            string hashedAccountId, string name, string code, string address, DateTime? incorporated,
            string legalEntityStatus, OrganisationType organisationType, bool? userIsAuthorisedToSign, string submit)
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
                Source = (short)organisationType
            };

            var response = await _orchestrator.CreateLegalEntity(request);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                response.Status = HttpStatusCode.OK;

                TempData["userNotAuthorised"] = "true";

                return View("ViewEntityAgreement", "EmployerAgreement", response);
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

            return RedirectToAction("Index", "EmployerAgreement", new { hashedAccountId });
        }



    }
}