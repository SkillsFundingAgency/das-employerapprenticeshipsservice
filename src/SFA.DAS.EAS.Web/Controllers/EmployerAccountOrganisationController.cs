using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/organisations")]
    public class EmployerAccountOrganisationController : BaseController
    {
        private readonly OrganisationOrchestrator _orchestrator;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public EmployerAccountOrganisationController(
            IOwinWrapper owinWrapper,
            OrganisationOrchestrator orchestrator,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            IMapper mapper,
            ILog logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage) 
            :base(owinWrapper, featureToggle,multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("custom/add")]
        public ActionResult AddCustomOrganisationDetails()
        {
            var response = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel()
            };

            return View("AddOtherOrganisationDetails", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("custom/add")]
        public async Task<ActionResult> AddOtherOrganisationDetails(OrganisationDetailsViewModel model)
        {
            var response = await _orchestrator.ValidateLegalEntityName(model);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                return View("AddOtherOrganisationDetails", response);
            }

            model.Type = OrganisationType.Other;

            var addressModel = new OrchestratorResponse<FindOrganisationAddressViewModel>
            {
                Data = _mapper.Map<FindOrganisationAddressViewModel>(response.Data)
            };

            return View("FindAddress", addressModel);
            
        }

        [HttpPost]
        [Route("address/find")]
        public ActionResult FindAddress(FindOrganisationAddressViewModel request)
        {
            var response = new OrchestratorResponse<FindOrganisationAddressViewModel>
            {
                Data = request,
                Status = HttpStatusCode.OK
            };

            if (!string.IsNullOrEmpty(request.OrganisationAddress))
            {
                var organisationDetailsViewModel = _orchestrator.StartConfirmOrganisationDetails(request);
                
                CreateOrganisationCookieData(organisationDetailsViewModel);

                return RedirectToAction("GatewayInform", "EmployerAccount");
            }

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("address/select")]
        public async Task<ActionResult> SelectAddress(FindOrganisationAddressViewModel request)
        {
            var response = await _orchestrator.GetAddressesFromPostcode(request);

            if (response?.Data?.Addresses != null && response.Data.Addresses.Count == 1)
            {
                var viewModel = _mapper.Map<AddOrganisationAddressViewModel>(request);

                viewModel.Address = response.Data.Addresses.Single();

                var addressResponse = new OrchestratorResponse<AddOrganisationAddressViewModel>
                {
                    Data = viewModel,
                    Status = HttpStatusCode.OK
                };

                return View("AddOrganisationAddress", addressResponse);
            }

            return View(response);
        }
        
        [HttpGet]
        [Route("address/update")]
        public ActionResult AddOrganisationAddress(AddOrganisationAddressViewModel request)
        {
            if (request.Address == null)
            {
                request.Address = new AddressViewModel();
            }

            var response = new OrchestratorResponse<AddOrganisationAddressViewModel>
            {
                Data = request,
                Status = HttpStatusCode.OK
            };

            return View("AddOrganisationAddress", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("address/update")]
        public ActionResult UpdateOrganisationAddress(AddOrganisationAddressViewModel request)
        {
            var response = _orchestrator.AddOrganisationAddress(request);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                request.Address = request.Address ?? new AddressViewModel();
                request.Address.ErrorDictionary = response.Data.ErrorDictionary;

                var errorResponse = new OrchestratorResponse<AddOrganisationAddressViewModel>
                {
                    Data = request,
                    Status = HttpStatusCode.BadRequest,
                    Exception = response.Exception,
                    FlashMessage = response.FlashMessage
                };

                return View("AddOrganisationAddress", errorResponse);
            }
            CreateOrganisationCookieData(response);

            return RedirectToAction("GatewayInform", "EmployerAccount", response.Data);
        }
        
        private async Task<OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>> FindPublicSectorOrganisation(string publicSectorOrganisationName, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.FindPublicSectorOrganisation(publicSectorOrganisationName, hashedAccountId, userIdClaim);
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
            return response;
        }

        private void CreateOrganisationCookieData(OrchestratorResponse<OrganisationDetailsViewModel> response)
        {
            EmployerAccountData data;
            if (response.Data?.Name != null)
            {
                data = new EmployerAccountData
                {
                    OrganisationType = response.Data.Type,
                    OrganisationReferenceNumber = response.Data.ReferenceNumber,
                    OrganisationName = response.Data.Name,
                    OrganisationDateOfInception = response.Data.DateOfInception,
                    OrganisationRegisteredAddress = response.Data.Address,
                    OrganisationStatus = response.Data.Status ?? string.Empty,
                    PublicSectorDataSource = response.Data.PublicSectorDataSource,
                    Sector = response.Data.Sector
                };
            }
            else
            {
                var existingData = _orchestrator.GetCookieData(HttpContext);

                data = new EmployerAccountData
                {
                    OrganisationType = existingData.OrganisationType,
                    OrganisationReferenceNumber = existingData.OrganisationReferenceNumber,
                    OrganisationName = existingData.OrganisationName,
                    OrganisationDateOfInception = existingData.OrganisationDateOfInception,
                    OrganisationRegisteredAddress = existingData.OrganisationRegisteredAddress,
                    OrganisationStatus = existingData.OrganisationStatus,
                    PublicSectorDataSource = existingData.PublicSectorDataSource,
                    Sector = existingData.Sector
                };
            }

            _orchestrator.CreateCookieData(HttpContext, data);
        }
    }
}