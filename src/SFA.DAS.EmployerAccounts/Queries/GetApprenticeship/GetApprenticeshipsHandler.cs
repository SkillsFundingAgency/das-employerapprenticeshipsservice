using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;

public class GetApprenticeshipsHandler : IRequestHandler<GetApprenticeshipsRequest, GetApprenticeshipsResponse>
{
    private readonly IValidator<GetApprenticeshipsRequest> _validator;
    private readonly ILogger<GetApprenticeshipsHandler> _logger;
    private readonly ICommitmentV2Service _commitmentV2Service;
    private readonly IEncodingService _encodingService;

    public GetApprenticeshipsHandler(
        IValidator<GetApprenticeshipsRequest> validator,
        ILogger<GetApprenticeshipsHandler> logger,
        ICommitmentV2Service commitmentV2Service,
        IEncodingService encodingService)
    {
        _validator = validator;
        _logger = logger;
        _commitmentV2Service = commitmentV2Service;
        _encodingService = encodingService;
    }

    public async Task<GetApprenticeshipsResponse> Handle(GetApprenticeshipsRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var accountId = _encodingService.Decode(message.HashedAccountId, EncodingType.AccountId);

        try
        {
            return new GetApprenticeshipsResponse
            {
                Apprenticeships = await _commitmentV2Service.GetApprenticeships(accountId)
            };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Failed to get Cohorts for {message.HashedAccountId}");
            return new GetApprenticeshipsResponse
            {
                HasFailed = true
            };
        }
    }
}