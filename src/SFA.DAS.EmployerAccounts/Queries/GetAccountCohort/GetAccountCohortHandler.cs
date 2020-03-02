using MediatR;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.Encoding;
using System.Linq;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountCohort
{
    public class GetAccountCohortHandler : IAsyncRequestHandler<GetAccountCohortRequest, GetAccountCohortResponse>
    {
        private readonly IValidator<GetAccountCohortRequest> _validator;
        private readonly ILog _logger;
        private readonly ICommitmentV2Service _commitmentV2Service;
        private readonly IEncodingService _encodingService;
        private readonly IHashingService _hashingService;

        public GetAccountCohortHandler(
            IValidator<GetAccountCohortRequest> validator,
            ILog logger,
            ICommitmentV2Service commitmentV2Service,
            IEncodingService encodingService,
            IHashingService hashingService)
        {
            _validator = validator;
            _logger = logger;
            _commitmentV2Service = commitmentV2Service;
            _encodingService = encodingService;
            _hashingService = hashingService;
        }


        public async Task<GetAccountCohortResponse> Handle(GetAccountCohortRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            long accountId = _hashingService.DecodeValue(message.HashedAccountId);

            _logger.Info($"Getting Cohorts for account id {message.HashedAccountId}");

            var cohortsResponse = await _commitmentV2Service.GetCohortsV2(accountId);
            var hashedCohortReference = _encodingService.Encode(cohortsResponse.First().CohortId, EncodingType.CohortReference);
            var hashedDraftApprenticeshipId = _encodingService.Encode(cohortsResponse.First().Apprenticeships.First().Id, EncodingType.ApprenticeshipId);

            return new GetAccountCohortResponse
            {
                CohortV2 = cohortsResponse,
                HashedCohortReference = hashedCohortReference,
                HashedDraftApprenticeshipId = hashedDraftApprenticeshipId
            };

        }
    }
}
