using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.Encoding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class CommitmentsV2Service : ICommitmentV2Service
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IMapper _mapper;
        private readonly IEncodingService _encodingService;

        public CommitmentsV2Service(ICommitmentsApiClient commitmentsApiClient, IMapper mapper, IEncodingService encodingService)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _mapper = mapper;
            _encodingService = encodingService;
        }

        public async Task<IEnumerable<Apprenticeship>> GetDraftApprenticeships(Cohort cohort)
        {
            var draftApprenticeshipsResponse = await _commitmentsApiClient.GetDraftApprenticeships(cohort.Id);
            return _mapper.Map<IEnumerable<DraftApprenticeshipDto>, IEnumerable<Apprenticeship>>(draftApprenticeshipsResponse.DraftApprenticeships,
               opt =>
               {
                   opt.AfterMap((src, dest) =>
                   {
                       dest.ToList().ForEach(c =>
                       {
                           c.SetHashId(_encodingService);
                           c.SetCohort(cohort);
                       });
                   });
               });
        }

        public async Task<IEnumerable<Cohort>> GetCohorts(long? accountId)
        {
            var cohortSummary = await _commitmentsApiClient.GetCohorts(new CommitmentsV2.Api.Types.Requests.GetCohortsRequest { AccountId = accountId });          

            return _mapper.Map<IEnumerable<CohortSummary>, IEnumerable<Cohort>>(cohortSummary.Cohorts,
                opt =>
                {   
                    opt.AfterMap((src, dest) =>
                    {
                        dest.ToList().ForEach(c =>
                        {
                            c.SetHashId(_encodingService);
                        });
                    });
                });
        }        

        public async Task<IEnumerable<Apprenticeship>> GetApprenticeships(long accountId)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeships(new CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest { AccountId = accountId });
            
            return _mapper.Map<IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>, IEnumerable<Apprenticeship>>(apprenticeship.Apprenticeships);
        }
     
    }
}
