﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetPostcodeAddress;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Application.Queries.GetTeamUser;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Web.Validation;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class OrganisationOrchestrator : UserVerificationOrchestratorBase, IOrchestratorCookie
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly ICookieStorageService<EmployerAccountData> _cookieService;
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        protected OrganisationOrchestrator()
        {
        }

        public OrganisationOrchestrator(IMediator mediator, ILog logger, IMapper mapper, ICookieStorageService<EmployerAccountData> cookieService)
            : base(mediator)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _cookieService = cookieService;
        }

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>>
            GetLimitedCompanyByRegistrationNumber(string companiesHouseNumber, string hashedLegalEntityId,
                string userIdClaim)
        {
            if (!string.IsNullOrEmpty(hashedLegalEntityId))
            {
                var result = await CheckLegalEntityIsNotAddedToAccount(hashedLegalEntityId, userIdClaim, companiesHouseNumber, OrganisationType.CompaniesHouse);

                if (result != null)
                {
                    result.Data.ErrorDictionary["CompaniesHouseNumber"] = "Company already added";
                    return result;
                }
            }

            var response = await _mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = companiesHouseNumber
            });

            if (response == null)
            {
                _logger.Warn("No response from SelectEmployerViewModel");
                var errorResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.Conflict
                };
                errorResponse.Data.ErrorDictionary["CompaniesHouseNumber"] = "Company not found";
                return errorResponse;
            }

            if (response.CompanyStatus != "active" && response.CompanyStatus != "administration" &&
                response.CompanyStatus != "voluntary-arrangement")
            {
                var errorResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.Conflict
                };
                errorResponse.Data.ErrorDictionary["CompaniesHouseNumber"] =
                    "Company must be active, under administration or in a voluntary arrangement";
                return errorResponse;
            }

            _logger.Info($"Returning Data for {companiesHouseNumber}");

            var address = BuildAddressString(response);

            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = hashedLegalEntityId,
                    Type = OrganisationType.CompaniesHouse,
                    ReferenceNumber = response.CompanyNumber,
                    Name = response.CompanyName,
                    DateOfInception =
                        response.DateOfIncorporation.Equals(DateTime.MinValue)
                            ? (DateTime?) null
                            : response.DateOfIncorporation,
                    Address = address,
                    Status = response.CompanyStatus
                }
            };
        }

        public virtual async Task<OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>>
            FindPublicSectorOrganisation(string searchTerm, string hashedAccountId, string userIdClaim)
        {
            var searchResults = await _mediator.SendAsync(new GetPublicSectorOrganisationQuery
            {
                SearchTerm = searchTerm,
                PageNumber = 1,
                PageSize = 200
            });

            if (searchResults == null || !searchResults.Organisaions.Data.Any())
            {
                _logger.Warn("No response from GetPublicSectorOrgainsationQuery");
                return new OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>
                {
                    Data = new PublicSectorOrganisationSearchResultsViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        SearchTerm = searchTerm,
                        Results = new PagedResponse<OrganisationDetailsViewModel>
                        {
                            Data = new List<OrganisationDetailsViewModel>()
                        }
                    },
                    Status = HttpStatusCode.OK
                };
            }
            
            var organisations = searchResults.Organisaions.Data.Select(x => new OrganisationDetailsViewModel
            {
                Name = x.Name,
                Status = string.Empty,
                Type = OrganisationType.PublicBodies,
                PublicSectorDataSource = (short)x.Source,
                Sector = x.Sector,
                Address = !string.IsNullOrEmpty(x.AddressLine1) && !string.IsNullOrEmpty(x.AddressLine4) && !string.IsNullOrEmpty(x.PostCode) 
                        ?_mediator.Send(new CreateOrganisationAddressRequest
                        {
                            AddressFirstLine = x.AddressLine1,
                            AddressSecondLine = x.AddressLine2,
                            AddressThirdLine =x.AddressLine3,
                            TownOrCity = x.AddressLine4,
                            County = x.AddressLine5,
                            Postcode = x.PostCode
                        }).Address : "",
                ReferenceNumber = x.OrganisationCode
                 
            }).ToList();

            if (!string.IsNullOrEmpty(hashedAccountId))
            {
                var accountLegalEntitiesHelper = new AccountLegalEntitiesHelper(Mediator);
                var accountEntities = await accountLegalEntitiesHelper.GetAccountLegalEntities(hashedAccountId, userIdClaim);

                foreach (var viewModel in organisations)
                {
                    viewModel.AddedToAccount = accountLegalEntitiesHelper.IsLegalEntityAlreadyAddedToAccount(accountEntities, viewModel.Name, null, OrganisationType.PublicBodies);
                }
            }
            
            var pagedResponse = new PagedResponse<OrganisationDetailsViewModel>
            {
                Data = organisations,
                PageNumber = searchResults.Organisaions.PageNumber,
                TotalPages = searchResults.Organisaions.TotalPages
            };

            return new OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>
            {
                Data = new PublicSectorOrganisationSearchResultsViewModel
                {
                    HashedAccountId = hashedAccountId,
                    SearchTerm = searchTerm,
                    Results = pagedResponse
                },
                Status = HttpStatusCode.OK
            };
        }

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> GetCharityByRegistrationNumber(
            string registrationNumber, string hashedLegalEntityId, string userIdClaim)
        {

            if (!string.IsNullOrEmpty(hashedLegalEntityId))
            {

                var result = await CheckLegalEntityIsNotAddedToAccount(hashedLegalEntityId, userIdClaim, registrationNumber, OrganisationType.Charities);

                if (result != null)
                {
                    result.Data.ErrorDictionary["CharityRegistrationNumber"] = "Charity already added";
                    return result;
                }
                
            }

            int charityRegistrationNumber;
            if (!int.TryParse(registrationNumber.Trim(), out charityRegistrationNumber))
            {
                var exception = new ArgumentException("Non-numeric registration number", nameof(registrationNumber));
                _logger.Error(exception, exception.Message);
            }

            GetCharityQueryResponse charityResult;
            try
            {
                charityResult = await _mediator.SendAsync(new GetCharityQueryRequest
                {
                    RegistrationNumber = charityRegistrationNumber
                });
            }
            catch (HttpRequestException)
            {
                _logger.Warn("Charity not found");
                var notFoundResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.NotFound
                };
                notFoundResponse.Data.ErrorDictionary["CharityRegistrationNumber"] = "Charity not found";
                return notFoundResponse;
            }

            if (charityResult == null)
            {
                _logger.Warn("No response from GetAccountLegalEntitiesRequest");
                var notFoundResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.NotFound
                };
                notFoundResponse.Data.ErrorDictionary["CharityRegistrationNumber"] = "Charity not found";
                return notFoundResponse;
            }

            var charity = charityResult.Charity;

            if (charity.IsRemoved)
            {
                var notFoundResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.BadRequest
                };
                notFoundResponse.Data.ErrorDictionary["CharityRegistrationNumber"] =
                    "Charity must have registered status";
                return notFoundResponse;
            }

            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = hashedLegalEntityId,
                    ReferenceNumber = charity.RegistrationNumber.ToString(),
                    Name = charity.Name,
                    Type = OrganisationType.Charities,
                    Address = charity.FormattedAddress,
                    Status = string.Empty
                }
            };
        }

        public virtual async Task<OrchestratorResponse<AddLegalEntityViewModel>> GetAddLegalEntityViewModel(
            string hashedAccountId, string externalUserId)
        {
            var userRole = await GetUserAccountRole(hashedAccountId, externalUserId);

            return new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel {HashedAccountId = hashedAccountId},
                Status = userRole.UserRole.Equals(Role.Owner) ? HttpStatusCode.OK : HttpStatusCode.Unauthorized
            };
        }

        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateLegalEntity(
            CreateNewLegalEntityViewModel request)
        {
            var createLegalEntityResponse = await _mediator.SendAsync(new CreateLegalEntityCommand
            {
                HashedAccountId = request.HashedAccountId,
                LegalEntity = new LegalEntity
                {
                    Name = request.Name,
                    Code = request.Code,
                    RegisteredAddress = request.Address,
                    DateOfIncorporation = request.IncorporatedDate,
                    Status = request.LegalEntityStatus,
                    Source = request.Source,
                    PublicSectorDataSource = request.PublicSectorDataSource,
                    Sector = request.Sector
                },
                ExternalUserId = request.ExternalUserId
            });

            return new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = createLegalEntityResponse.AgreementView
                },
                Status = HttpStatusCode.OK
            };
        }

        public virtual OrchestratorResponse<OrganisationDetailsViewModel> GetAddOtherOrganisationViewModel(
            string hashedAccountId)
        {
            var response = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = hashedAccountId
                }
            };

            return response;
        }

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> ValidateLegalEntityName(
            OrganisationDetailsViewModel request)
        {
            var response = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = request
            };

            var validator = new OrganisationDetailsViewModelValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                response.Data.ErrorDictionary = new Dictionary<string, string>();
                foreach (var validationError in validationResult.Errors)
                {
                    response.Data.ErrorDictionary.Add(validationError.PropertyName, validationError.ErrorMessage);
                }

                response.Status = HttpStatusCode.BadRequest;

                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    Severity = FlashMessageSeverityLevel.Error,
                    ErrorMessages = response.Data.ErrorDictionary
                };
            }

            return response;
        }

        public virtual OrchestratorResponse<OrganisationDetailsViewModel> AddOrganisationAddress(
            AddOrganisationAddressViewModel viewModel)
        {
            try
            {
                var request = _mapper.Map<CreateOrganisationAddressRequest>(viewModel.Address);

                var response = _mediator.Send(request);

                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel
                    {
                        HashedId = viewModel.OrganisationHashedId,
                        Name = viewModel.OrganisationName,
                        Address = response.Address,
                        DateOfInception = viewModel.OrganisationDateOfInception,
                        ReferenceNumber = viewModel.OrganisationReferenceNumber ?? string.Empty,
                        Type = viewModel.OrganisationType,
                        PublicSectorDataSource = viewModel.PublicSectorDataSource,
                        Status = viewModel.OrganisationStatus,
                        Sector = viewModel.Sector
                    }
                };
            }
            catch (InvalidRequestException e)
            {
                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel
                    {
                        ErrorDictionary = e.ErrorMessages
                    },
                    Status = HttpStatusCode.BadRequest,
                    Exception = e,
                    FlashMessage = new FlashMessageViewModel
                    {
                        Headline = "Errors to fix",
                        Message = "Check the following details:",
                        ErrorMessages = e.ErrorMessages,
                        Severity = FlashMessageSeverityLevel.Error
                    }
                };
            }
        }

        public virtual OrchestratorResponse<AddOrganisationAddressViewModel>
            CreateAddOrganisationAddressViewModelFromOrganisationDetails(OrganisationDetailsViewModel model)
        {
            var result = new OrchestratorResponse<AddOrganisationAddressViewModel>
            {
                Data = new AddOrganisationAddressViewModel
                {
                    OrganisationType = OrganisationType.Other,
                    OrganisationHashedId = model.HashedId,
                    OrganisationName = model.Name
                }
            };
            return result;
        }

        public virtual EmployerAccountData GetCookieData(HttpContextBase context)
        {
            return _cookieService.Get(CookieName);
        }

        public virtual void CreateCookieData(HttpContextBase context, EmployerAccountData data)
        {
            _cookieService.Create(data,CookieName, 365);
        }

        public virtual async Task<OrchestratorResponse<SelectOrganisationAddressViewModel>> GetAddressesFromPostcode(
            FindOrganisationAddressViewModel request)
        {
            var viewModel = new SelectOrganisationAddressViewModel
            {
                Postcode = request.Postcode,
                OrganisationName = request.OrganisationName,
                OrganisationDateOfInception = request.OrganisationDateOfInception,
                OrganisationHashedId = request.OrganisationHashedId,
                OrganisationReferenceNumber = request.OrganisationReferenceNumber,
                OrganisationStatus = request.OrganisationStatus,
                OrganisationType = request.OrganisationType,
                PublicSectorDataSource = request.PublicSectorDataSource,
                Sector = request.Sector
            };

            try
            {
                var addresses = await _mediator.SendAsync(new GetPostcodeAddressRequest {Postcode = request.Postcode});

                viewModel.Addresses = addresses?.Addresses?.Select(x => _mapper.Map<AddressViewModel>(x)).ToList();

                return new OrchestratorResponse<SelectOrganisationAddressViewModel>
                {
                    Data = viewModel,
                    Status = HttpStatusCode.OK
                };
            }
            catch (InvalidRequestException e)
            {
                viewModel.ErrorDictionary = e.ErrorMessages;

                return new OrchestratorResponse<SelectOrganisationAddressViewModel>
                {
                    Data = viewModel,
                    Status = HttpStatusCode.BadRequest,
                    Exception = e
                };
            }
        }

        private static string BuildAddressString(GetEmployerInformationResponse response)
        {
            var addressBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(response.AddressLine1))
            {
                addressBuilder.Append($"{response.AddressLine1}, ");
            }

            if (!string.IsNullOrEmpty(response.AddressLine2))
            {
                addressBuilder.Append($"{response.AddressLine2}, ");
            }

            if (!string.IsNullOrEmpty(response.TownOrCity))
            {
                addressBuilder.Append($"{response.TownOrCity}, ");
            }

            if (!string.IsNullOrEmpty(response.County))
            {
                addressBuilder.Append(string.IsNullOrEmpty(response.AddressPostcode)
                    ? $"{response.County}"
                    : $"{response.County}, ");
            }

            if (!string.IsNullOrEmpty(response.AddressPostcode))
            {
                addressBuilder.Append(response.AddressPostcode);
            }

            return addressBuilder.ToString();
        }

        public async Task<OrchestratorResponse<OrganisationDetailsViewModel>> CheckLegalEntityIsNotAddedToAccount(string hashedLegalEntityId, string userIdClaim, string legalEntityCode, OrganisationType organisationType)
        {
            var accountLegalEntitiesHelper = new AccountLegalEntitiesHelper(_mediator);
            var accountEntities = await accountLegalEntitiesHelper.GetAccountLegalEntities(hashedLegalEntityId, userIdClaim);

            if(accountLegalEntitiesHelper.IsLegalEntityAlreadyAddedToAccount(accountEntities, null, legalEntityCode, organisationType))
            {
                var conflictResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.Conflict
                };
                
                return conflictResponse;
            }

            return null;
        }

        public OrchestratorResponse<OrganisationDetailsViewModel> StartConfirmOrganisationDetails(AddOrganisationAddressViewModel request)
        {
            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = request.OrganisationHashedId,
                    Name = request.OrganisationName,
                    Address = request.OrganisationAddress,
                    DateOfInception = request.OrganisationDateOfInception,
                    ReferenceNumber = request.OrganisationReferenceNumber ?? string.Empty,
                    Type = request.OrganisationType,
                    PublicSectorDataSource = request.PublicSectorDataSource,
                    Status = request.OrganisationStatus,
                    Sector = request.Sector
                }
            };
        }

        public OrchestratorResponse<OrganisationDetailsViewModel> StartConfirmOrganisationDetails(FindOrganisationAddressViewModel request)
        {
            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = request.OrganisationHashedId,
                    Name = request.OrganisationName,
                    Address = request.OrganisationAddress,
                    DateOfInception = request.OrganisationDateOfInception,
                    ReferenceNumber = request.OrganisationReferenceNumber ?? string.Empty,
                    Type = request.OrganisationType,
                    PublicSectorDataSource = request.PublicSectorDataSource,
                    Status = request.OrganisationStatus,
                    Sector = request.Sector
                }
            };
        }

        public virtual Task<OrchestratorResponse<OrganisationAddedNextStepsViewModel>> GetOrganisationAddedNextStepViewModel(
                string organisationName,
                string userId,
                string hashedAccountId)
        {
            return this.GetOrganisationAddedNextStepViewModel(organisationName, userId, hashedAccountId, string.Empty);
        }

        public virtual async Task<OrchestratorResponse<OrganisationAddedNextStepsViewModel>> GetOrganisationAddedNextStepViewModel(
            string organisationName, 
            string userId, 
            string hashedAccountId, 
            string hashedAgreementId)
        {
            var showWizard = await UserShownWizard(userId, hashedAccountId);

            return new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
            {
                Data = new OrganisationAddedNextStepsViewModel { OrganisationName = organisationName, ShowWizard = showWizard, HashedAgreementId = hashedAgreementId}
            };
        }

        public virtual async Task<bool> UserShownWizard(string userId, string hashedAccountId)
        {
            var userResponse = await Mediator.SendAsync(new GetTeamMemberQuery { HashedAccountId = hashedAccountId, TeamMemberId = userId });
            return userResponse.User.ShowWizard && userResponse.User.RoleId == (short)Role.Owner;
        }
    }
}