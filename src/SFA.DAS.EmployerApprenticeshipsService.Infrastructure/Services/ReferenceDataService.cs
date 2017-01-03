using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.ReferenceData.Api.Client;


namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
        private const int DefaultPageSize = 100;

        private readonly IReferenceDataApiClient _client;
        private readonly IMapper _mapper;

        public ReferenceDataService(IReferenceDataApiClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
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
    }
}
