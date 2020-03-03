using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Commitments;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class CommitmentsV2Service : ICommitmentV2Service
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IMapper _mapper;

        public CommitmentsV2Service(ICommitmentsApiClient commitmentsApiClient, IMapper mapper)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _mapper = mapper;
        }

        public async Task<GetApprenticeshipsResponse> GetApprenticeship(long? accountId)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeships
                (new SFA.DAS.CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest { AccountId = accountId });
            return apprenticeship;
        }

        public async Task<GetCohortsResponse> GetCohorts(long? accountId)
        {
            var cohorts = await _commitmentsApiClient.GetCohorts(new CommitmentsV2.Api.Types.Requests.GetCohortsRequest { AccountId = accountId });
            if (cohorts == null )
            {
                return new GetCohortsResponse(new List<CohortSummary>());
            }
            return cohorts;
        }
       

        public async  Task<IEnumerable<CohortV2>> GetCohortsV2(long? accountId, CohortFilter cohortFilter)
        {
            if (cohortFilter.Take != 1) { throw new System.Exception("Not Supported"); }
            var cohortsResponse = new List<CohortV2>() { new CohortV2() { Apprenticeships = new List<Apprenticeship> { } } };
            var apprenticeship = await _commitmentsApiClient.GetApprenticeships(new CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest() { AccountId = accountId });
            if (apprenticeship?.TotalApprenticeshipsFound >= cohortFilter.Take)
            {
                cohortsResponse.First().Apprenticeships = _mapper.Map<ICollection<Apprenticeship>>(apprenticeship.Apprenticeships);

                return cohortsResponse;
            }
            else
            {
                var cohortSummary = await _commitmentsApiClient.GetCohorts(new CommitmentsV2.Api.Types.Requests.GetCohortsRequest { AccountId = accountId });

                if (cohortSummary != null && cohortSummary.Cohorts != null && cohortSummary.Cohorts.Any())
                {
                    var mappedCohorts = _mapper.Map<IEnumerable<CohortSummary>, IEnumerable<CohortV2>>(cohortSummary.Cohorts);

                    if (mappedCohorts.Count() <= cohortFilter.Take)
                    {                        
                        foreach (var cohort in mappedCohorts)
                        {
                            if (cohort.NumberOfDraftApprentices == 1)
                            {
                                var draftApprenticeshipsResponse = await _commitmentsApiClient.GetDraftApprenticeships(cohort.CohortId);
                                cohortsResponse.First().CohortId = cohort.CohortId;
                                cohortsResponse.First().CohortStatus = cohort.CohortStatus;
                                cohortsResponse.First().NumberOfDraftApprentices = cohort.NumberOfDraftApprentices;
                                cohortsResponse.First().Apprenticeships = _mapper.Map<IEnumerable<DraftApprenticeshipDto>, List<Apprenticeship>>(draftApprenticeshipsResponse.DraftApprenticeships);                                                     
                            }
                        }
                    }
                }
            }

            return cohortsResponse;
        }

        public CohortStatus GetStatus(CohortSummary cohort)
        {
            if (cohort.IsDraft && cohort.WithParty == Party.Employer)
                return CohortStatus.Draft;
            else if (!cohort.IsDraft && cohort.WithParty == Party.Employer)
                return CohortStatus.Review;
            else if (!cohort.IsDraft && cohort.WithParty == Party.Provider)
                return CohortStatus.WithTrainingProvider;
            else if (!cohort.IsDraft && cohort.WithParty == Party.TransferSender)
                return CohortStatus.WithTransferSender;
            else
                return CohortStatus.Unknown;
        }

        public async Task<GetDraftApprenticeshipsResponse> GetDraftApprenticeships(long cohortId)
        {
            var draftApprenticeshipsResponse = await _commitmentsApiClient.GetDraftApprenticeships(cohortId);
            if (draftApprenticeshipsResponse == null)
            {
                return new GetDraftApprenticeshipsResponse();
            }
            
            return draftApprenticeshipsResponse;
        }

     
    }
}
