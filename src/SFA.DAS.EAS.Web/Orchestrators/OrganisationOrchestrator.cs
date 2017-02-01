using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetLatestEmployerAgreementTemplate;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class OrganisationOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ICookieService _cookieService;
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        protected OrganisationOrchestrator()
        {
        }

        public OrganisationOrchestrator(IMediator mediator, ILogger logger, IMapper mapper, ICookieService cookieService) : base(mediator)
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
	            var accountEntities = await GetAccountLegalEntities(hashedLegalEntityId, userIdClaim);

	            if (accountEntities.Entites.LegalEntityList.Any(x =>
                    (!string.IsNullOrWhiteSpace(x.Code)
                    && x.Code.Equals(companiesHouseNumber, StringComparison.CurrentCultureIgnoreCase))))
                {
	                var errorResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
	                {
	                    Data = new OrganisationDetailsViewModel(),
	                    Status = HttpStatusCode.Conflict
	                };
	                errorResponse.Data.ErrorDictionary["CompaniesHouseNumber"] = "Company already added";
	                return errorResponse;
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
                errorResponse.Data.ErrorDictionary["CompaniesHouseNumber"] = "Company must be active, under administration or in a voluntary arrangement";
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
                    DateOfInception = response.DateOfIncorporation.Equals(DateTime.MinValue) ? (DateTime?)null : response.DateOfIncorporation,
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
                PublicSectorDataSource = (short)x.Source
            }).ToList();

            if (!string.IsNullOrEmpty(hashedAccountId))
            {
                var accountEntities = await GetAccountLegalEntities(hashedAccountId, userIdClaim);

                foreach (var viewModel in organisations)
                {
                    viewModel.AddedToAccount = accountEntities.Entites.LegalEntityList.Any(
                        e => e.Name.Equals(viewModel.Name, StringComparison.CurrentCultureIgnoreCase));
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
	            var accountEntities = await GetAccountLegalEntities(hashedLegalEntityId, userIdClaim);

	            if (accountEntities.Entites.LegalEntityList.Any(
	                x =>(!String.IsNullOrWhiteSpace(x.Code) && x.Code.Equals(registrationNumber, StringComparison.CurrentCultureIgnoreCase))
	                && x.Source == (short)OrganisationType.Charities))
	            {
	                var conflictResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
	                {
	                    Data = new OrganisationDetailsViewModel(),
	                    Status = HttpStatusCode.Conflict
	                };
	                conflictResponse.Data.ErrorDictionary["CharityRegistrationNumber"] = "Charity already added";
	                return conflictResponse;
	            }
			}

            int charityRegistrationNumber;
            if (!int.TryParse(registrationNumber.Trim(), out charityRegistrationNumber))
            {
                _logger.Error("Non-numeric registration number");
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
                notFoundResponse.Data.ErrorDictionary["CharityRegistrationNumber"] = "Charity must have registered status";
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
        
        private async Task<GetAccountLegalEntitiesResponse> GetAccountLegalEntities(string hashedLegalEntityId,
            string userIdClaim)
        {
            var accountEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedLegalEntityId = hashedLegalEntityId,
                UserId = userIdClaim
            });
            return accountEntities;
        }

        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateLegalEntity(
            CreateNewLegalEntity request)
        {
            if (request.SignedAgreement && !request.UserIsAuthorisedToSign)
            {
                var response = await _mediator.SendAsync(new GetLatestEmployerAgreementTemplateRequest
                {
                    HashedAccountId = request.HashedAccountId,
                    UserId = request.ExternalUserId
                });

                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = new EmployerAgreementView
                        {
                            LegalEntityName = request.Name,
                            LegalEntityCode = request.Code,
                            LegalEntityAddress = request.Address,
                            LegalEntityInceptionDate = request.IncorporatedDate,
                            Status = EmployerAgreementStatus.Pending,
                            TemplateRef = response.Template.Ref,
                            TemplateText = response.Template.Text,
                            LegalEntityStatus = request.LegalEntityStatus
                        }
                    },
                    Status = HttpStatusCode.BadRequest
                };
            }

            var createLegalEntityResponse = await _mediator.SendAsync(new CreateLegalEntityCommand
            {
                HashedAccountId = request.HashedAccountId,
                LegalEntity = new LegalEntity
                {
                    Name = request.Name,
                    Code = request.Code,
                    RegisteredAddress = request.Address,
                    DateOfIncorporation = request.IncorporatedDate,
                    CompanyStatus = request.LegalEntityStatus,
                    Source = request.Source,
                    PublicSectorDataSource = request.PublicSectorDataSource
                },
                SignAgreement = request.UserIsAuthorisedToSign && request.SignedAgreement,
                SignedDate = request.SignedDate,
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

        public virtual OrchestratorResponse<OrganisationDetailsViewModel> GetAddOtherOrganisationViewModel(string hashedAccountId)
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

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> ValidateLegalEntityName(OrganisationDetailsViewModel request)
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
            AddOrganisationAddressModel model)
        {
            try
            {
                var request = _mapper.Map<CreateOrganisationAddressRequest>(model);

                var response = _mediator.Send(request);

                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel
                    {
                        HashedId = model.OrganisationHashedId,
                        Name = model.OrganisationName,
                        Address = response.Address,
                        DateOfInception = model.OrganisationDateOfInception,
                        ReferenceNumber = model.OrganisationReferenceNumber,
                        Type = model.OrganisationType,
                        PublicSectorDataSource = model.PublicSectorDataSource,
                        Status = model.OrganisationStatus
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

        public virtual OrchestratorResponse<AddOrganisationAddressModel> CreateAddOrganisationAddressViewModelFromOrganisationDetails(OrganisationDetailsViewModel model)
        {
            var result = new OrchestratorResponse<AddOrganisationAddressModel>
            {
                Data = new AddOrganisationAddressModel
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
            var cookie = (string)_cookieService.Get(context, CookieName);

            if (string.IsNullOrEmpty(cookie))
                return null;

            return JsonConvert.DeserializeObject<EmployerAccountData>(cookie);
        }

        public virtual void CreateCookieData(HttpContextBase context, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            _cookieService.Create(context, CookieName, json, 365);
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
    }
}