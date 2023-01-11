using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;

public class GetApprenticeshipsHandler : IAsyncRequestHandler<GetApprenticeshipsRequest, GetApprenticeshipsResponse>
{
    private readonly IValidator<GetApprenticeshipsRequest> _validator;
    private readonly ILog _logger;
    private readonly ICommitmentV2Service _commitmentV2Service;
    private readonly IHashingService _hashingService;

    public GetApprenticeshipsHandler(
        IValidator<GetApprenticeshipsRequest> validator,
        ILog logger,
        ICommitmentV2Service commitmentV2Service,
        IHashingService hashingService)
    {
        _validator = validator;
        _logger = logger;
        _commitmentV2Service = commitmentV2Service;
        _hashingService = hashingService;
    }

    public async Task<GetApprenticeshipsResponse> Handle(GetApprenticeshipsRequest message)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        long accountId = _hashingService.DecodeValue(message.HashedAccountId);

        try
        {
            return new GetApprenticeshipsResponse
            {
                Apprenticeships = await _commitmentV2Service.GetApprenticeships(accountId)
            };
        }
        catch(Exception ex)
        {
            _logger.Error(ex, $"Failed to get Cohorts for {message.HashedAccountId}");
            return new GetApprenticeshipsResponse
            {
                HasFailed = true
            };
        }
    }
}