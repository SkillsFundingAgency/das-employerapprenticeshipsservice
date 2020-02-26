using MediatR;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship
{
    public class GetApprenticeshipHandler : IAsyncRequestHandler<GetApprenticeshipRequest, GetApprenticeshipResponse>
    {
        private readonly IValidator<GetApprenticeshipRequest> _validator;
        private readonly ILog _logger;        
        private readonly ICommitmentV2Service _commitmentV2Service;

        public GetApprenticeshipHandler(
            IValidator<GetApprenticeshipRequest> validator,
            ILog logger,
            ICommitmentV2Service commitmentV2Service)
        {
            _validator = validator;
            _logger = logger;            
            _commitmentV2Service = commitmentV2Service;
        }
        
        public async Task<GetApprenticeshipResponse> Handle(GetApprenticeshipRequest message)
        {
            var validationResult = _validator.Validate(message);

            if(!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            _logger.Info($"Getting Apprentices for account id {message.AccountId}");

            return new GetApprenticeshipResponse
            {
                ApprenticeshipDetailsResponse = await _commitmentV2Service.GetApprenticeship(message.AccountId)
            };            
        }
    }
}
