using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Domain;
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

        public virtual async Task<OrchestratorResponse<FindOrganisationViewModel>> FindLegalEntity(string hashedLegalEntityId, OrganisationType orgType,
           string searchTerm, string userIdClaim)
        {
            var accountEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedLegalEntityId = hashedLegalEntityId,
                UserId = userIdClaim
            });


            //todo: if code is the same AND of the same type
            if (accountEntities.Entites.LegalEntityList.Any(
                x => x.Code.Equals(searchTerm, StringComparison.CurrentCultureIgnoreCase)))
            {
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindOrganisationViewModel(),
                    Status = HttpStatusCode.Conflict
                };
            }

            switch (orgType)
            {
                case OrganisationType.Charities:
                    return await FindCharity(hashedLegalEntityId, searchTerm);
                case OrganisationType.CompaniesHouse:
                    return await FindLimitedCompany(hashedLegalEntityId, searchTerm);
                case OrganisationType.PublicBodies:
                    return await FindPublicBody(hashedLegalEntityId, searchTerm);
                default:
                    throw new NotImplementedException("Organisation Type Not Implemented");
            }



        }

        private async Task<OrchestratorResponse<FindOrganisationViewModel>> FindLimitedCompany(string hashedLegalEntityId, string companiesHouseNumber)
        {

            var response = await _mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = companiesHouseNumber
            });

            if (response == null)
            {
                _logger.Warn("No response from SelectEmployerViewModel");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindOrganisationViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            _logger.Info($"Returning Data for {companiesHouseNumber}");

            return new OrchestratorResponse<FindOrganisationViewModel>
            {
                Data = new FindOrganisationViewModel
                {
                    HashedLegalEntityId = hashedLegalEntityId,
                    CompanyNumber = response.CompanyNumber,
                    CompanyName = response.CompanyName,
                    DateOfIncorporation = response.DateOfIncorporation,
                    RegisteredAddress = $"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}",
                    CompanyStatus = response.CompanyStatus,
                    Source = OrganisationType.CompaniesHouse
                }
            };
        }


        private async Task<OrchestratorResponse<FindOrganisationViewModel>> FindPublicBody(string hashedLegalEntityId, string searchTerm)
        {
            var publicBodyResult = await _mediator.SendAsync(new GetPublicSectorOrganisationQuery()
            {
                SearchTerm = searchTerm,
                PageNumber = 1,
                PageSize = 1000
            });

            if (publicBodyResult == null || !publicBodyResult.Organisaions.Data.Any())
            {
                _logger.Warn("No response from GetPublicSectorOrgainsationQuery");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindPublicBodyViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            var publicBody = publicBodyResult.Organisaions.Data.First();

            return new OrchestratorResponse<FindOrganisationViewModel>
            {
                Data = new FindPublicBodyViewModel
                {
                    Results = publicBodyResult.Organisaions,
                    CompanyName = publicBody.Name,
                    Source = OrganisationType.PublicBodies
                },
                Status = HttpStatusCode.OK
            };

        }

        private async Task<OrchestratorResponse<FindOrganisationViewModel>> FindCharity(string hashedLegalEntityId, string registrationNumber)
        {
            int charityRegistrationNumber;
            if (!Int32.TryParse(registrationNumber, out charityRegistrationNumber))
            {
                _logger.Warn("Non-numeric registration number");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindCharityViewModel(),
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
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindCharityViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            if (charityResult == null)
            {
                _logger.Warn("No response from GetAccountLegalEntitiesRequest");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindCharityViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            var charity = charityResult.Charity;

            if (charity.IsRemoved)
            {
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindCharityViewModel
                    {
                        IsRemovedError = true
                    },
                    Status = HttpStatusCode.NotFound
                };
            }

            return new OrchestratorResponse<FindOrganisationViewModel>
            {
                Data = new FindCharityViewModel
                {
                    HashedLegalEntityId = hashedLegalEntityId,
                    CompanyNumber = charity.RegistrationNumber.ToString(),
                    CompanyName = charity.Name,
                    RegisteredAddress = $"{charity.Address1}, {charity.Address2}, {charity.Address3}, {charity.Address4}, {charity.Address5}",
                    Source = OrganisationType.Charities
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
    }
}