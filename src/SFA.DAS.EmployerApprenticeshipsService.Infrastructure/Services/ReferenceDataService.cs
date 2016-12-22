//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AutoMapper;
//using SFA.DAS.EAS.Domain.Interfaces;
//using SFA.DAS.EAS.Domain.Models.ReferenceData;
//using SFA.DAS.ReferenceData.Api.Client;


//namespace SFA.DAS.EAS.Infrastructure.Services
//{
//    public class ReferenceDataService : IReferenceDataService
//    {
//        private readonly IReferenceDataApiClient _client;

//        public ReferenceDataService(IReferenceDataApiClient client)
//        {
//            _client = client;
//        }

//        public async Task<Charity> GetCharity(int registrationNumber)
//        {
//            var dto = await _client.GetCharity(registrationNumber);
//            var result = Mapper.Map<ReferenceData.Api.Client.Dto.Charity, Charity>(dto);
//            return result;
//        }

//        //public async Task<PagedApiResponse<PublicSectorOrganisation>> SearchPublicSectorOrganisation(string searchTerm, int pageNumber, int pageSize)
//        //{
//        //    var dto = await _client.
//        //}

//    }
//}
