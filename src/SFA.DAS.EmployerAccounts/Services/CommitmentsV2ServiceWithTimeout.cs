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
using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Exceptions;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class CommitmentsV2ServiceWithTimeout : ICommitmentV2Service
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IMapper _mapper;
        private readonly IEncodingService _encodingService;
        private readonly IAsyncPolicy _pollyPolicy;

        public CommitmentsV2ServiceWithTimeout(ICommitmentsApiClient commitmentsApiClient, IMapper mapper,
            IEncodingService encodingService,
            IReadOnlyPolicyRegistry<string> pollyRegistry)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _mapper = mapper;
            _encodingService = encodingService;
            _pollyPolicy = pollyRegistry.Get<IAsyncPolicy>(Constants.DefaultServiceTimeout);
        }

        public async Task<IEnumerable<Apprenticeship>> GetDraftApprenticeships(Cohort cohort)
        {
            try
            {
                var draftApprenticeshipsResponse = await _pollyPolicy.ExecuteAsync(() => _commitmentsApiClient.GetDraftApprenticeships(cohort.Id));
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
            catch (TimeoutRejectedException ex)
            {
                throw new ServiceTimeoutException("Call to Commitments V2 Service timed out", ex);
            }
        }

        public async Task<IEnumerable<Cohort>> GetCohorts(long? accountId)
        {
            try
            {
                var cohortSummary = await _pollyPolicy.ExecuteAsync(() => _commitmentsApiClient.GetCohorts(new CommitmentsV2.Api.Types.Requests.GetCohortsRequest { AccountId = accountId }));
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
            catch (TimeoutRejectedException ex)
            {
                throw new ServiceTimeoutException("Call to Commitments V2 Service timed out", ex);
            }
        }

        public async Task<IEnumerable<Apprenticeship>> GetApprenticeships(long accountId)
        {
            try
            {
                var apprenticeship = await _pollyPolicy.ExecuteAsync(() => _commitmentsApiClient.GetApprenticeships(new CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest { AccountId = accountId }));
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
            catch (TimeoutRejectedException ex)
            {
                throw new ServiceTimeoutException("Call to Commitments V2 Service timed out", ex);
            }
        }
    }
}
