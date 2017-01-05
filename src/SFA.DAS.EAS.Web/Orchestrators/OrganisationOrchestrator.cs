using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetLatestEmployerAgreementTemplate;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class OrganisationOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        protected OrganisationOrchestrator()
        {
        }

        public OrganisationOrchestrator(IMediator mediator, ILogger logger): base(mediator)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> GetLimitedCompanyByRegistrationNumber(string companiesHouseNumber, string hashedLegalEntityId, string userIdClaim)
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
        
        public virtual async Task<OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>> FindPublicSectorOrganisation( string searchTerm, string hashedLegalEntityId, string userIdClaim)
        {
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
                    Data = new PublicSectorOrganisationSearchResultsViewModel(),
                    Status = HttpStatusCode.OK
                };
            }

            var pagedResponse = new PagedResponse<OrganisationDetailsViewModel>
            {
                Data =
                    searchResults.Organisaions.Data.Select(x => new OrganisationDetailsViewModel {Name = x.Name})
                        .ToList(),
                PageNumber = searchResults.Organisaions.PageNumber,
                TotalPages = searchResults.Organisaions.TotalPages
            };

            return new OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>
            {
                Data = new PublicSectorOrganisationSearchResultsViewModel
                {
                    Results = pagedResponse
                },
                Status = HttpStatusCode.OK
            };
        }

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> GetCharityByRegistrationNumber(string registrationNumber, string hashedLegalEntityId, string userIdClaim)
        {
            var accountEntities = await GetAccountLegalEntities(hashedLegalEntityId, userIdClaim);

            if (accountEntities.Entites.LegalEntityList.Any(
               x => x.Code.Equals(registrationNumber, StringComparison.CurrentCultureIgnoreCase)))
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
                    Address = $"{charity.Address1}, {charity.Address2}, {charity.Address3}, {charity.Address4}, {charity.Address5}"
                }
            };
        }

        public async Task<OrchestratorResponse<AddLegalEntityViewModel>> GetAddLegalEntityViewModel(string hashedAccountId, string externalUserId)
        {
            var userRole = await GetUserAccountRole(hashedAccountId, externalUserId);

            return new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel { HashedAccountId = hashedAccountId },
                Status = userRole.UserRole.Equals(Role.Owner) ? HttpStatusCode.OK : HttpStatusCode.Unauthorized
            };
        }

        private async Task<GetAccountLegalEntitiesResponse> GetAccountLegalEntities(string hashedLegalEntityId, string userIdClaim)
        {
            var accountEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedLegalEntityId = hashedLegalEntityId,
                UserId = userIdClaim
            });
            return accountEntities;
        }


        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateLegalEntity(CreateNewLegalEntity request)
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

    }
}