using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.Validation;


namespace SFA.DAS.EmployerAccounts.Queries.GetSingleDraftApprenticeship
{
    public class GetSingleDraftApprenticeshipRequestHandler : IAsyncRequestHandler<GetSingleDraftApprenticeshipRequest, GetSingleDraftApprenticeshipResponse>
    {
        private readonly IValidator<GetSingleDraftApprenticeshipRequest> _validator;
        private readonly ILog _logger;
        private readonly ICommitmentV2Service _commitmentV2Service;
        private readonly IEncodingService _encodingService;

        public GetSingleDraftApprenticeshipRequestHandler(
              IValidator<GetSingleDraftApprenticeshipRequest> validator,
              ILog logger,
              ICommitmentV2Service commitmentV2Service,
              IEncodingService encodingService)
        {
            _validator = validator;
            _logger = logger;
            _commitmentV2Service = commitmentV2Service;
            _encodingService = encodingService;
        }

        
        public async Task<GetSingleDraftApprenticeshipResponse> Handle(GetSingleDraftApprenticeshipRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            _logger.Info($"Getting Draft Apprentices for cohort id {message.CohortId}");


            var draftApprenticeshipsResponse = await _commitmentV2Service.GetDraftApprenticeships(message.CohortId);                      

            return new GetSingleDraftApprenticeshipResponse
            {
                
            };
        }
    }
}
