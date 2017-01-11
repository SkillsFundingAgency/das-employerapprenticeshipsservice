using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
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

        protected OrganisationOrchestrator()
        {
        }

        public OrganisationOrchestrator(IMediator mediator, ILogger logger, IMapper mapper) : base(mediator)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>>
            GetLimitedCompanyByRegistrationNumber(string companiesHouseNumber, string hashedLegalEntityId,
                string userIdClaim)
        {
            var accountEntities = await GetAccountLegalEntities(hashedLegalEntityId, userIdClaim);

            if (accountEntities.Entites.LegalEntityList.Any(
                x => x.Code.Equals(companiesHouseNumber, StringComparison.CurrentCultureIgnoreCase)))
            {
                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.Conflict
                };
            }

            var response = await _mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = companiesHouseNumber
            });

            if (response == null)
            {
                _logger.Warn("No response from SelectEmployerViewModel");
                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            _logger.Info($"Returning Data for {companiesHouseNumber}");

            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = hashedLegalEntityId,
                    ReferenceNumber = response.CompanyNumber,
                    Name = response.CompanyName,
                    DateOfInception = response.DateOfIncorporation,
                    Address = $"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}",
                    Status = response.CompanyStatus
                }
            };
        }

        public virtual async Task<OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>>
            FindPublicSectorOrganisation(string searchTerm, string hashedAccountId, string userIdClaim)
        {
            var accountEntities = await GetAccountLegalEntities(hashedAccountId, userIdClaim);
            
            var searchResults = await _mediator.SendAsync(new GetPublicSectorOrganisationQuery
            {
                SearchTerm = searchTerm,
                PageNumber = 1,
                PageSize = 1000
            });

            if (searchResults == null || !searchResults.Organisaions.Data.Any())
            {
                _logger.Warn("No response from GetPublicSectorOrgainsationQuery");
                return new OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>
                {
                    Data = new PublicSectorOrganisationSearchResultsViewModel
                    {
                        HashedAccountId = hashedAccountId,
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
                AddedToAccount =
                    accountEntities.Entites.LegalEntityList.Any(
                        e => e.Name.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase))
            }).ToList();

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
                    Results = pagedResponse
                },
                Status = HttpStatusCode.OK
            };
        }

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> GetCharityByRegistrationNumber(
            string registrationNumber, string hashedLegalEntityId, string userIdClaim)
        {
            var accountEntities = await GetAccountLegalEntities(hashedLegalEntityId, userIdClaim);

            if (accountEntities.Entites.LegalEntityList.Any(
                x => x.Code.Equals(registrationNumber, StringComparison.CurrentCultureIgnoreCase)
                && x.Source == (short)OrganisationType.Charities))
            {
                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.Conflict
                };
            }

            int charityRegistrationNumber;
            if (!int.TryParse(registrationNumber, out charityRegistrationNumber))
            {
                _logger.Warn("Non-numeric registration number");
                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.NotFound
                };
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
                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            if (charityResult == null)
            {
                _logger.Warn("No response from GetAccountLegalEntitiesRequest");
                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            var charity = charityResult.Charity;

            if (charity.IsRemoved)
            {
                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Status = HttpStatusCode.BadRequest
                };
            }

            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = hashedLegalEntityId,
                    ReferenceNumber = charity.RegistrationNumber.ToString(),
                    Name = charity.Name,
                    Type = OrganisationType.Charities,
                    Address =
                        $"{charity.Address1}, {charity.Address2}, {charity.Address3}, {charity.Address4}, {charity.Address5}"
                }
            };
        }

        public async Task<OrchestratorResponse<AddLegalEntityViewModel>> GetAddLegalEntityViewModel(
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

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateLegalEntity(
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
                            LegalEntityRegisteredAddress = request.Address,
                            LegalEntityIncorporatedDate = request.IncorporatedDate,
                            Status = EmployerAgreementStatus.Pending,
                            TemplateRef = response.Template.Ref,
                            TemplateText = response.Template.Text,
                            LegalEntityStatus = request.LegalEntityStatus,

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
                    Source = request.Source
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

        public OrchestratorResponse<OrganisationDetailsViewModel> GetAddOtherOrganisationViewModel(string hashedAccountId)
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

        public async Task<OrchestratorResponse<OrganisationDetailsViewModel>> ValidateLegalEntityName(OrganisationDetailsViewModel request)
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

        public OrchestratorResponse<OrganisationDetailsViewModel> AddOrganisationAddress(
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

        public OrchestratorResponse<AddOrganisationAddressModel> CreateAddOrganisationAddressViewModelFromOrganisationDetails(OrganisationDetailsViewModel model)
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
    }
}