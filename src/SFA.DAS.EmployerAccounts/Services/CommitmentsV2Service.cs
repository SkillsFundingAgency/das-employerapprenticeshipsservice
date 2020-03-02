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


        public async Task<IEnumerable<CohortV2>> GetCohortsV2(long? accountId)
        {            
            var cohortsResponse = new List<CohortV2>()  {  new CohortV2()  { Apprenticeships = new List<Apprenticeship> { } } };            
            var apprenticeship = await _commitmentsApiClient.GetApprenticeships(new CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest() { AccountId = accountId });
            if (apprenticeship?.TotalApprenticeshipsFound > 0)
            {                
                cohortsResponse.First().Apprenticeships = new List<Apprenticeship>()
                {
                    new Apprenticeship()
                    {
                        CourseName = apprenticeship.Apprenticeships.First().CourseName,
                        CourseStartDate = apprenticeship.Apprenticeships.First().StartDate,
                        CourseEndDate = apprenticeship.Apprenticeships.First().EndDate,
                        FirstName = apprenticeship.Apprenticeships.First().FirstName,
                        LastName = apprenticeship.Apprenticeships.First().LastName,
                        ApprenticeshipStatus = Models.Commitments.ApprenticeshipStatus.Approved,
                        TrainingProvider = new TrainingProvider
                        {
                            Name = apprenticeship.Apprenticeships.First().ProviderName
                        }
                    }
                };
            }
            else
            {
                var cohorts = await _commitmentsApiClient.GetCohorts(new CommitmentsV2.Api.Types.Requests.GetCohortsRequest { AccountId = accountId });
                if (cohorts == null) { return new List<CohortV2>() { new CohortV2 { Apprenticeships = new List<Apprenticeship>() } }; };

                if (cohorts.Cohorts != null && cohorts.Cohorts.Count() == 1)
                {
                    var singleCohort = cohorts.Cohorts.First();
                    if (singleCohort.NumberOfDraftApprentices == 1)
                    {
                        cohortsResponse = new List<CohortV2>()
                        {
                            new CohortV2()
                            {
                                CohortId = singleCohort.CohortId,
                                CohortsCount = cohorts.Cohorts.Count(),
                                NumberOfDraftApprentices = singleCohort.NumberOfDraftApprentices,
                                CohortStatus = GetStatus(singleCohort)
                            }
                        };

                        var draftApprenticeshipsResponse = await _commitmentsApiClient.GetDraftApprenticeships(singleCohort.CohortId);
                        if (draftApprenticeshipsResponse == null) { return new List<CohortV2>() { new CohortV2 { Apprenticeships = new List<Apprenticeship>() } }; };
                        var singleDraftApprentice = draftApprenticeshipsResponse?.DraftApprenticeships?.First();                        

                        cohortsResponse.First().Apprenticeships = new List<Apprenticeship>()
                        {
                            new Apprenticeship()
                            {
                                FirstName = singleDraftApprentice.FirstName,
                                LastName = singleDraftApprentice.LastName,
                                CourseName = singleDraftApprentice.CourseName,
                                CourseStartDate = singleDraftApprentice.StartDate,
                                CourseEndDate = singleDraftApprentice.EndDate,
                                ApprenticeshipStatus = Models.Commitments.ApprenticeshipStatus.Draft,
                                TrainingProvider = new TrainingProvider()
                                {
                                    Id = singleCohort.ProviderId,
                                    Name = singleCohort.ProviderName
                                }
                            },
                        };
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
