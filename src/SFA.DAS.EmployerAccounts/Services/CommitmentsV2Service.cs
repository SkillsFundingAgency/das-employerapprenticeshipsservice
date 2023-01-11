using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Services;

public class CommitmentsV2Service : ICommitmentV2Service
{
    private readonly ICommitmentsV2ApiClient _commitmentsApiClient;
    private readonly IMapper _mapper;
    private readonly IEncodingService _encodingService;

    public CommitmentsV2Service(ICommitmentsV2ApiClient commitmentsApiClient, IMapper mapper, IEncodingService encodingService)
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
                        c.SetTrainingProvider(cohort.TrainingProvider.First());
                    });
                });
            });
    }

    public async Task<IEnumerable<Cohort>> GetCohorts(long? accountId)
    {
        var cohortSummary = await _commitmentsApiClient.GetCohorts(new CommitmentsV2.Api.Types.Requests.GetCohortsRequest { AccountId = accountId });
        var trainingProvider = _mapper.Map<IEnumerable<CohortSummary>, IEnumerable<TrainingProvider>>(cohortSummary.Cohorts);

        return _mapper.Map<IEnumerable<CohortSummary>, IEnumerable<Cohort>>(cohortSummary.Cohorts,
            opt =>
            {   
                opt.AfterMap((src, dest) =>
                {
                    dest.ToList().ForEach(c =>
                    {
                        c.SetHashId(_encodingService);
                        c.SetTrainingProvider(trainingProvider);
                    });                       
                });
            });
    }        

    public async Task<IEnumerable<Apprenticeship>> GetApprenticeships(long accountId)
    {
        var apprenticeship = await _commitmentsApiClient.GetApprenticeships(new CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest { AccountId = accountId });
        var trainingProvider = _mapper.Map<IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>, IEnumerable<TrainingProvider>>(apprenticeship.Apprenticeships);

        return _mapper.Map<IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>, IEnumerable<Apprenticeship>>(apprenticeship.Apprenticeships,
            opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    dest.ToList().ForEach(c =>
                    {
                        c.SetHashId(_encodingService);
                        c.SetTrainingProvider(trainingProvider.First());
                    });
                });
            });
    }

    public async Task<List<Cohort>> GetEmployerCommitments(long employerAccountId)
    {            
        var request = new GetCohortsRequest { AccountId = employerAccountId };
        var commitmentItems = await _commitmentsApiClient.GetCohorts(request);

        if (commitmentItems == null || !commitmentItems.Cohorts.Any())
        {
            return new List<Cohort>();
        }

        return commitmentItems.Cohorts.Where(x => x.CommitmentStatus != CommitmentStatus.Deleted)
            .Select(x => new Cohort { Id = x.CohortId }).ToList();
    }
}