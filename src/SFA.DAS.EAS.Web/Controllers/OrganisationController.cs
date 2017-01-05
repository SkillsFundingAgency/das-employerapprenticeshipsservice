using System;
using System.Linq;
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
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class OrganisationController : BaseController
    {
        private readonly OrganisationOrchestrator _orchestrator;

        public OrganisationController(
            IOwinWrapper owinWrapper, 
            OrganisationOrchestrator orchestrator,
            IFeatureToggle featureToggle, 
            IUserWhiteList userWhiteList) 
            :base(owinWrapper, featureToggle, userWhiteList)
        {
            _orchestrator = orchestrator;
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
            var errorResponse = new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel { HashedAccountId = hashedAccountId },
                Status = HttpStatusCode.OK,
            };

            switch (orgType)
            {
                case OrganisationType.Charities:
                    var charityResponse = await FindCharity(charityRegNo, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

                    if (charityResponse.Status == HttpStatusCode.OK)
                    {
                        return View("ConfirmCharityDetails", charityResponse);
                    }

                    break;
                case OrganisationType.CompaniesHouse:
                    var companyResponse = await FindCompany(companiesHouseNumber, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

                    if (companyResponse.Status == HttpStatusCode.OK)
                    {
                        return View("ConfirmCompanyDetails", companyResponse);
                    }

                    break;
                case OrganisationType.PublicBodies:
                    var publicSectorOrganisationSearchResponse = await FindPublicSectorOrganisation(publicBodyName, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

                    if (publicSectorOrganisationSearchResponse.Data.Results.Data.Count == 1)
                    {
                        var model = publicSectorOrganisationSearchResponse.Data.Results.Data.First();

                        return View("ConfirmPublicSectorOrganisationDetails",
                            new OrchestratorResponse<OrganisationDetailsViewModel>()
                            {
                                Data = new OrganisationDetailsViewModel {Name = model.Name},
                                Status = HttpStatusCode.OK
                            });
                    }

                    return View("ViewPublicSectorOrganisationSearchResults", publicSectorOrganisationSearchResponse);
                    
                case OrganisationType.Other:
                    return View("AddCustomOrganisationDetails", new OrchestratorResponse<OrganisationDetailsViewModel>());
                    
                default:
                    throw new NotImplementedException("Org Type Not Implemented");
            }

            return View("AddOrganisation", errorResponse);
        }

        private async Task<OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>> FindPublicSectorOrganisation(string publicSectorOrganisationName, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.FindPublicSectorOrganisation(publicSectorOrganisationName, hashedAccountId, userIdClaim);

            return response;
        }

        private async Task<OrchestratorResponse<CompanyDetailsViewModel>> FindCompany(string companiesHouseNumber, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.GetLimitedCompanyByRegistrationNumber(companiesHouseNumber, hashedAccountId, userIdClaim);

            if (response.Status == HttpStatusCode.NotFound)
            {
                TempData["companyError"] = "Company not found";
            }

            if (response.Status == HttpStatusCode.Conflict)
            {
                TempData["companyError"] = "Enter a company that isn't already registered";
                
            }

            return response;
        }

        private async Task<OrchestratorResponse<CharityDetailsViewModel>> FindCharity(string charityRegNo, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.GetCharityByRegistrationNumber(charityRegNo, hashedAccountId, userIdClaim);

            if (response.Status == HttpStatusCode.NotFound)
            {
                TempData["charityError"] = "Charity not found";
            }

            if (response.Data.IsRemovedError)
            {
                TempData["charityError"] = "Charity is removed";
                response.Status = HttpStatusCode.BadRequest;
            }

            return response;

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Route("Agreements/CreateAgreement")]
        //public async Task<ActionResult> CreateLegalEntity(
        //    string hashedAccountId, string name, string code, string address, DateTime? incorporated,
        //    bool? userIsAuthorisedToSign, string submit, string legalEntityStatus, OrganisationType legalEntitySource)
        //{
        //    var request = new CreateNewLegalEntity
        //    {
        //        HashedAccountId = hashedAccountId,
        //        Name = name,
        //        Code = code,
        //        Address = address,
        //        IncorporatedDate = incorporated,
        //        UserIsAuthorisedToSign = userIsAuthorisedToSign ?? false,
        //        SignedAgreement = submit.Equals("Sign", StringComparison.CurrentCultureIgnoreCase),
        //        SignedDate = DateTime.Now,
        //        ExternalUserId = OwinWrapper.GetClaimValue(@"sub"),
        //        LegalEntityStatus = legalEntityStatus,
        //        Source = (short)legalEntitySource
        //    };

        //    var response = await _orchestrator.CreateLegalEntity(request);

        //    if (response.Status == HttpStatusCode.BadRequest)
        //    {
        //        response.Status = HttpStatusCode.OK;

        //        TempData["userNotAuthorised"] = "true";

        //        return View("ViewEntityAgreement", "EmployerAgreement", response);
        //    }

        //    TempData["extraCompanyAdded"] = "true";

        //    if (request.UserIsAuthorisedToSign && request.SignedAgreement)
        //    {
        //        TempData["successHeader"] = $"{response.Data.EmployerAgreement.LegalEntityName} has been added";
        //        TempData["successMessage"] = "This account can now spend levy funds.";
        //    }
        //    else
        //    {
        //        TempData["successHeader"] = $"{response.Data.EmployerAgreement.LegalEntityName} has been added";
        //        TempData["successMessage"] = "To spend the levy funds somebody needs to sign the agreement.";
        //    }

        //    return RedirectToAction("Index", "EmployerAgreement", new { hashedAccountId });
        //}


    }
}