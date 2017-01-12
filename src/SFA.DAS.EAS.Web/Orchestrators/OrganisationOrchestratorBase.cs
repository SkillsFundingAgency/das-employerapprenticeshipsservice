using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class OrganisationOrchestratorBase : UserVerificationOrchestratorBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public OrganisationOrchestratorBase(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> GetCompany(string registrationNumber)
        {
            var response = await _mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = registrationNumber
            });

            if (response == null)
            {
                _logger.Warn("No response from Companies House");
                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            _logger.Info($"Returning Data for {registrationNumber}");

            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    ReferenceNumber = response.CompanyNumber,
                    Name = response.CompanyName,
                    DateOfInception = response.DateOfIncorporation,
                    Address = $"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}",
                    Status = response.CompanyStatus
                }
            };
        }

        public virtual async Task<OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>> FindPublicSectorOrganisation(string searchTerm)
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
                    Data = new PublicSectorOrganisationSearchResultsViewModel
                    {
                        Results = new PagedResponse<OrganisationDetailsViewModel>
                        {
                            Data = new List<OrganisationDetailsViewModel>()
                        }
                    },
                    Status = HttpStatusCode.OK
                };
            }

            var organisations = searchResults.Organisaions.Data.Select(x => 
                new OrganisationDetailsViewModel { Name = x.Name }).ToList();

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
                    Results = pagedResponse
                },
                Status = HttpStatusCode.OK
            };
        }

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> GetCharity(string registrationNumber)
        {
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
                    ReferenceNumber = charity.RegistrationNumber.ToString(),
                    Name = charity.Name,
                    Type = OrganisationType.Charities,
                    Address = $"{charity.Address1}, {charity.Address2}, {charity.Address3}, {charity.Address4}, {charity.Address5}"
                }
            };
        }
    }
}