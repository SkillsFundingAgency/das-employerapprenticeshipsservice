using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.ReferenceData.Api.Client;


namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
        private const int DefaultPageSize = 100;

        private readonly IReferenceDataApiClient _client;
        private readonly IMapper _mapper;
        private readonly ICacheProvider _cacheProvider;

        public ReferenceDataService(IReferenceDataApiClient client, IMapper mapper, ICacheProvider cacheProvider)
        {
            _client = client;
            _mapper = mapper;
            _cacheProvider = cacheProvider;
        }

        public async Task<Charity> GetCharity(int registrationNumber)
        {
            var dto = await _client.GetCharity(registrationNumber);
            var result = _mapper.Map<ReferenceData.Api.Client.Dto.Charity, Charity>(dto);
            return result;
        }

        public Task<PagedResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(string searchTerm)
        {
            return SearchPublicSectorOrganisation(searchTerm, 1, DefaultPageSize);
        }

        public Task<PagedResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(string searchTerm, int pageNumber)
        {
            return SearchPublicSectorOrganisation(searchTerm, pageNumber, DefaultPageSize);
        }

        public async Task<PagedResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(string searchTerm, int pageNumber, int pageSize)
        {
            var dto = await _client.SearchPublicSectorOrganisation(searchTerm, pageNumber, pageSize);

            var orgainsations = dto.Data.Select(x => _mapper.Map<PublicSectorOrganisation>(x)).ToList();

            return new PagedResponse<PublicSectorOrganisation>
            {
                Data = orgainsations,
                PageNumber = dto.PageNumber,
                TotalPages = dto.TotalPages
            };
        }

        public async Task<PagedResponse<Organisation>> SearchOrganisations(string searchTerm, int pageNumber = 1, int pageSize = 25, OrganisationType? organisationType = null)
        {
            var result = await SearchOrganisations(searchTerm);

            if (result == null)
            {
                return new PagedResponse<Organisation>();
            }

            if (organisationType != null)
            {
                result = FilterOrganisationsByType(result, organisationType.Value);
            }
            
            return CreatePagedOrganisationResponse(pageNumber, pageSize, result);
        }

        private List<Organisation> FilterOrganisationsByType(IEnumerable<Organisation> result, OrganisationType organisationType)
        {
            if (organisationType == OrganisationType.Other || organisationType == OrganisationType.PublicBodies)
            {
                return result.Where(x => x.Type == OrganisationType.Other || x.Type == OrganisationType.PublicBodies).ToList();
            }
            return result.Where(x => x.Type == organisationType).ToList();
        }

        private async Task<List<Organisation>> SearchOrganisations(string searchTerm)
        {
            var cacheKey = $"SearchKey_{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(searchTerm))}";

            var result = _cacheProvider.Get<List<Organisation>>(cacheKey);
            if (result == null)
            {
                var orgs = await _client.SearchOrganisations(searchTerm);

                if (orgs != null)
                {
                    result = orgs.Select(ConvertToOrganisation).ToList();
                    _cacheProvider.Set(cacheKey, result, new TimeSpan(0, 15, 0));
                }
            }
            return result;
        }

        private static PagedResponse<Organisation> CreatePagedOrganisationResponse(int pageNumber, int pageSize, List<Organisation> result)
        {
            return new PagedResponse<Organisation>
            {
                Data = result.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList(),
                TotalPages = (int)Math.Ceiling(((decimal)(result.Count / pageSize))),
                PageNumber = pageNumber,
                TotalResults = result.Count
            };
        }

        private Organisation ConvertToOrganisation(ReferenceData.Api.Client.Dto.Organisation source)
        {
            return new Organisation
            {
                Address = new Address
                {
                    Line1 = source.Address.Line1,
                    Line2 = source.Address.Line2,
                    Line3 = source.Address.Line3,
                    Line4 = source.Address.Line4,
                    Line5 = source.Address.Line5,
                    Postcode = source.Address.Postcode
                },
                Name = source.Name,
                Code = source.Code,
                RegistrationDate = source.RegistrationDate,
                Sector = source.Sector,
                SubType = ConvertToOrganisationSubType(source.SubType),
                Type = ConvertToOrganisationType(source.Type)
            };
        }

        private OrganisationType ConvertToOrganisationType(ReferenceData.Api.Client.Dto.OrganisationType sourceType)
        {
            switch (sourceType)
            {
                case ReferenceData.Api.Client.Dto.OrganisationType.Charity:
                    return OrganisationType.Charities;
                case ReferenceData.Api.Client.Dto.OrganisationType.Company:
                    return OrganisationType.CompaniesHouse;
                case ReferenceData.Api.Client.Dto.OrganisationType.EducationOrganisation:
                    return OrganisationType.Other;
                case ReferenceData.Api.Client.Dto.OrganisationType.PublicSector:
                    return OrganisationType.PublicBodies;
                default:
                    return OrganisationType.Other;
            }
        }

        private OrganisationSubType ConvertToOrganisationSubType(ReferenceData.Api.Client.Dto.OrganisationSubType sourceSubType)
        {
            switch (sourceSubType)
            {
                case ReferenceData.Api.Client.Dto.OrganisationSubType.Nhs:
                    return OrganisationSubType.Nhs;
                case ReferenceData.Api.Client.Dto.OrganisationSubType.Ons:
                    return OrganisationSubType.Ons;
                case ReferenceData.Api.Client.Dto.OrganisationSubType.Police:
                    return OrganisationSubType.Police;
                default:
                    return 0;
            }
        }
    }
}
