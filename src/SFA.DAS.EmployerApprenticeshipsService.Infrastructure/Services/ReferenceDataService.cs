using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.ReferenceData.Api.Client;


namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
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



        //public async Task<PagedApiResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(string searchTerm, int pageNumber, int pageSize)
        //{
        //    var dto = await _client.SearchPublicSectorOrganisation(searchTerm, pageNumber, pageSize);

        //    var orgainsations = dto.Data.Select(x => new PublicSectorOrganisation { Name = x.Name }).ToList();

        //    return new PagedApiResponse<PublicSectorOrganisation>
        //    {
        //        Data = orgainsations,
        //        PageNumber = dto.PageNumber,
        //        TotalPages = dto.TotalPages
        //    };
        //}

    }
}
