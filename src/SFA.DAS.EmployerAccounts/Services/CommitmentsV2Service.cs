using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Commitments;
using SFA.DAS.Encoding;
using System.Collections.Generic;
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



        public async Task<IEnumerable<Apprenticeship>> GetDraftApprenticeships(long cohortId)
        {
            var draftApprenticeshipsResponse = await _commitmentsApiClient.GetDraftApprenticeships(cohortId);

            return _mapper.Map<IEnumerable<DraftApprenticeshipDto>, List<Apprenticeship>>(draftApprenticeshipsResponse.DraftApprenticeships,
                opt =>
                {
                    opt.AfterMap((src, dest) =>
                    {
                        foreach (var apprenticeship in dest)
                        {
                            apprenticeship.HashedId = _encodingService.Encode(apprenticeship.Id, EncodingType.ApprenticeshipId);
                        }
                    });

                });
        }


        public async Task<IEnumerable<CohortV2>> GetCohortsV2(long? accountId)
        {
            var cohortSummary = await _commitmentsApiClient.GetCohorts(new CommitmentsV2.Api.Types.Requests.GetCohortsRequest { AccountId = accountId });

            //return _mapper.Map<IEnumerable<CohortSummary>, IEnumerable<CohortV2>>(cohortSummary.Cohorts);

            return _mapper.Map<IEnumerable<CohortSummary>, IEnumerable<CohortV2>>(cohortSummary.Cohorts,
                opt =>
                {
                    opt.AfterMap((src, dest) =>
                        {
                            foreach (var cohort in dest)
                            {
                                cohort.HashedId = _encodingService.Encode(cohort.Id, EncodingType.CohortReference);
                            }
                        });
                });

        }

        public async Task<IEnumerable<Apprenticeship>> GetApprenticeships(long accountId)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeships
                (new CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest { AccountId = accountId });

            return _mapper.Map<ICollection<Apprenticeship>>(apprenticeship.Apprenticeships); 
        }
    }
}
