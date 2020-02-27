using MediatR;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.Encoding;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Queries.GetCohorts
{
    public class GetCohortsHandler : IAsyncRequestHandler<GetCohortsRequest, GetCohortsResponse>
    {
        private readonly IValidator<GetCohortsRequest> _validator;
        private readonly ILog _logger;        
        private readonly ICommitmentV2Service _commitmentV2Service;
        private readonly IEncodingService _encodingService;


        public GetCohortsHandler(
            IValidator<GetCohortsRequest> validator,
            ILog logger,
            ICommitmentV2Service commitmentV2Service,            
            IEncodingService encodingService)
        {
            _validator = validator;
            _logger = logger;            
            _commitmentV2Service = commitmentV2Service;
            _encodingService = encodingService;
        }

        public async Task<GetCohortsResponse> Handle(GetCohortsRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            _logger.Info($"Getting Cohorts for account id {message.AccountId}");

            var cohortsResponse = await _commitmentV2Service.GetCohorts(message.AccountId);
            var hashedCohortReference = _encodingService.Encode(cohortsResponse.Cohorts.First().CohortId, EncodingType.CohortReference);
            var singleCohort = cohortsResponse.Cohorts.First();

            return new GetCohortsResponse
            {
                CohortsResponse = cohortsResponse,
                HashedCohortReference = hashedCohortReference,
                SingleCohort = singleCohort                
            };
        }
    }
}
