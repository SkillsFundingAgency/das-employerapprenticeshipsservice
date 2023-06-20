using System.Threading;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;

public class GetApprenticeshipsHandler : IRequestHandler<GetApprenticeshipsRequest, GetApprenticeshipsResponse>
{
    private readonly IValidator<GetApprenticeshipsRequest> _validator;
    private readonly ILogger<GetApprenticeshipsHandler> _logger;
    private readonly ICommitmentV2Service _commitmentV2Service;

    public GetApprenticeshipsHandler(
        IValidator<GetApprenticeshipsRequest> validator,
        ILogger<GetApprenticeshipsHandler> logger,
        ICommitmentV2Service commitmentV2Service)
    {
        _validator = validator;
        _logger = logger;
        _commitmentV2Service = commitmentV2Service;
    }

    public async Task<GetApprenticeshipsResponse> Handle(GetApprenticeshipsRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        try
        {
            return new GetApprenticeshipsResponse
            {
                Apprenticeships = await _commitmentV2Service.GetApprenticeships(message.AccountId)
            };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to get Cohorts for AccountId: {AccountId}", message.AccountId);
            return new GetApprenticeshipsResponse
            {
                HasFailed = true
            };
        }
    }
}