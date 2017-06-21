using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.EAS.Domain.Interfaces;
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

        public async Task<PagedResponse<Organisation>> SearchOrganisations(string searchTerm,int pageNumber = 1, int pageSize = 25)
        {
            var cacheKey = $"SearchKey_{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(searchTerm))}";

            var result = _cacheProvider.Get<List<Organisation>>(cacheKey);
            if (result == null)
            {
                var orgs = await _client.SearchOrganisations(searchTerm);

                if (orgs != null)
                {
                    result = orgs.Select(x => _mapper.Map<Organisation>(x)).ToList();
                    _cacheProvider.Set(cacheKey, result,new TimeSpan(0,15,0));
                    
                }
            }

            if (result == null)
            {
                return new PagedResponse<Organisation>();
            }

            return new PagedResponse<Organisation>
            {
                Data = result.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList(),
                TotalPages = ((result.Count+9) / pageSize) + 1,
                PageNumber = pageNumber
            };
        }
    }
}
