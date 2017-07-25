﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}/organisations")]
    public class OrganisationController : BaseController
    {
        private readonly OrganisationOrchestrator _orchestrator;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public OrganisationController(
            IOwinWrapper owinWrapper, 
            OrganisationOrchestrator orchestrator,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            IMapper mapper,
            ILog logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage) 
            :base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("add")]
        public async Task<ActionResult> AddOrganisation(string hashedAccountId)
        {
            var response = await _orchestrator.GetAddLegalEntityViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add")]
        public async Task<ActionResult> AddOrganisation(AddLegalEntityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var orgTypeErrorResponse = new OrchestratorResponse<AddLegalEntityViewModel>
                {
                    Data = model
                };

                orgTypeErrorResponse.Data.AddErrorsFromModelState(ModelState);

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

            OrchestratorResponse<OrganisationDetailsViewModel> response;

            //TODO this can be removed once we determine which is slow in prod
            var stopwatch = new System.Diagnostics.Stopwatch();
            try
            {
                stopwatch.Start();

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

                        if (searchResponse.Status == HttpStatusCode.OK)
                        {
                            if (searchResponse.Data.Results.Data.Count != 1 ||
                                searchResponse.Data.Results.Data.All(x => x.AddedToAccount))
                            {
                                return View("ViewPublicSectorOrganisationSearchResults", searchResponse);
                            }

                            response = new OrchestratorResponse<OrganisationDetailsViewModel>
                            {
                                Data = searchResponse.Data.Results.Data.FirstOrDefault(),
                                Status = searchResponse.Status
                            };
                        }
                        else
                        {
                            response = new OrchestratorResponse<OrganisationDetailsViewModel>
                            {
                                Data = new OrganisationDetailsViewModel(),
                                Status = searchResponse.Status
                            };
                            response.Data.ErrorDictionary = searchResponse.Data.ErrorDictionary;
                        }

                        break;

                    case OrganisationType.Other:
                        return RedirectToAction("AddOtherOrganisationDetails", "Organisation", new { model.HashedAccountId });
                    
                    default:
                        throw new NotImplementedException("Org Type Not Implemented");
                }
            }
            finally
            {
                stopwatch.Stop();
                _logger.Info($"Company Search for {model.OrganisationType} took {stopwatch.ElapsedMilliseconds / 1000d}s");
            }

            if (response.Status == HttpStatusCode.OK)
            {
                //Removes empty address entries (i.e. ' , , ')
                var address = (response.Data?.Address ?? string.Empty).Replace(",", string.Empty);

                if (string.IsNullOrWhiteSpace(address))
                {
                    var addressViewModel = _mapper.Map<FindOrganisationAddressViewModel>(response.Data);

                    var addressResponse = new OrchestratorResponse<FindOrganisationAddressViewModel>
                    {
                        Data = addressViewModel
                    };

                    return View("FindAddress", addressResponse);
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

        [HttpPost]
        [Route("address/find")]
        public ActionResult FindAddress(FindOrganisationAddressViewModel request)
        {
            var response = new OrchestratorResponse<FindOrganisationAddressViewModel>
            {
                Data = request,
                Status = HttpStatusCode.OK
            };


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

            if (!string.IsNullOrEmpty(request.OrganisationAddress))
            {
                var organisationDetailsViewModel = _orchestrator.StartConfirmOrganisationDetails(request);
                return View("ConfirmOrganisationDetails", organisationDetailsViewModel);
            }


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

            return View("ConfirmOrganisationDetails", response);
        }

        [HttpGet]
        [Route("custom/add")]
        public ActionResult AddOtherOrganisationDetails(string hashedAccountId)
        {
            var response = _orchestrator.GetAddOtherOrganisationViewModel(hashedAccountId);
            return View(response);
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
        [ValidateAntiForgeryToken]
        [Route("legalAgreement")]
        public ActionResult OrganisationLegalAgreement(string hashedAccountId, OrganisationDetailsViewModel model)
        {
            var viewModel = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = model,
                Status = HttpStatusCode.OK
            };

            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirm")]
        public async Task<ActionResult> Confirm(
            string hashedAccountId, string name, string code, string address, DateTime? incorporated,
            string legalEntityStatus, OrganisationType organisationType, short? publicSectorDataSource, string sector, bool newSearch)
        {
            var request = new CreateNewLegalEntityViewModel
            {
                HashedAccountId = hashedAccountId,
                Name = name,
                Code = code,
                Address = address,
                IncorporatedDate = incorporated,
                ExternalUserId = OwinWrapper.GetClaimValue(@"sub"),
                LegalEntityStatus = string.IsNullOrWhiteSpace(legalEntityStatus) ? null : legalEntityStatus,
                Source = (short)organisationType,
                PublicSectorDataSource = publicSectorDataSource,
                Sector = sector
            };

            var response = await _orchestrator.CreateLegalEntity(request);
            
            var flashMessage = new FlashMessageViewModel
            {
                HiddenFlashMessageInformation = "page-organisations-added",
                Headline = $"{response.Data.EmployerAgreement.LegalEntityName} has been added",
                Severity = FlashMessageSeverityLevel.Success
            };
            AddFlashMessageToCookie(flashMessage);
            if (newSearch)
            {
                return RedirectToAction("OrganisationAddedNextStepsSearch", new { hashedAccountId, organisationName = name });
            }
            return RedirectToAction("OrganisationAddedNextSteps", new { hashedAccountId, organisationName = name });
        }

        [HttpGet]
        [Route("nextStep")]
        public async Task<ActionResult> OrganisationAddedNextSteps(string organisationName, string hashedAccountId)
        {
            var userId = OwinWrapper.GetClaimValue(@"sub");

            var viewModel = await _orchestrator.GetOrganisationAddedNextStepViewModel(organisationName, userId, hashedAccountId);

            viewModel.FlashMessage = GetFlashMessageViewModelFromCookie();

            return View(viewModel);
        }

        [HttpGet]
        [Route("nextStepSearch")]
        public async Task<ActionResult> OrganisationAddedNextStepsSearch(string organisationName, string hashedAccountId)
        {
            var userId = OwinWrapper.GetClaimValue(@"sub");

            var viewModel = await _orchestrator.GetOrganisationAddedNextStepViewModel(organisationName, userId, hashedAccountId);

            viewModel.FlashMessage = GetFlashMessageViewModelFromCookie();

            return View("OrganisationAddedNextSteps", viewModel);
        }


        [HttpPost]
        [Route("nextStep")]
        public async Task<ActionResult> GoToNextStep(string nextStep, string hashedAccountId, string organisationName)
        {
            var userId = OwinWrapper.GetClaimValue(@"sub");

            var userShownWizard = await _orchestrator.UserShownWizard(userId, hashedAccountId);

            switch (nextStep)
            {
                case "agreement": return RedirectToAction("Index", "EmployerAgreement", new { hashedAccountId });

                case "teamMembers": return RedirectToAction("ViewTeam", "EmployerTeam", new { hashedAccountId });

                case "addOrganisation": return RedirectToAction("SearchForOrganisation", "SearchOrganisation", new { hashedAccountId });

                case "dashboard": return RedirectToAction("Index", "EmployerTeam", new { hashedAccountId });

                default:
                    var errorMessage = "Please select one of the next steps below";
                    return View("OrganisationAddedNextSteps", new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
                    {
                        Data = new OrganisationAddedNextStepsViewModel { ErrorMessage = errorMessage, OrganisationName = organisationName, ShowWizard = userShownWizard },
                        FlashMessage = new FlashMessageViewModel
                        {
                            Headline = "Invalid next step chosen",
                            Message = errorMessage,
                            ErrorMessages = new Dictionary<string, string> { { "nextStep", errorMessage } },
                            Severity = FlashMessageSeverityLevel.Error
                        }
                    });
            }
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
    }
}