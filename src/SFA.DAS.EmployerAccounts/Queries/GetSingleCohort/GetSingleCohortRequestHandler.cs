using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;

public class GetSingleCohortRequestHandler : IRequestHandler<GetSingleCohortRequest, GetSingleCohortResponse>
{
    private readonly IValidator<GetSingleCohortRequest> _validator;
    private readonly ICommitmentV2Service _commitmentV2Service;
    private readonly IHashingService _hashingService;
    private readonly ILogger<GetSingleCohortRequestHandler> _logger;

    public GetSingleCohortRequestHandler(
        IValidator<GetSingleCohortRequest> validator,
        ICommitmentV2Service commitmentV2Service,            
        IHashingService hashingService,
        ILogger<GetSingleCohortRequestHandler> logger)
    {
        _validator = validator;
        _commitmentV2Service = commitmentV2Service;
        _hashingService = hashingService;
        _logger = logger;
    }

    public async Task<GetSingleCohortResponse> Handle(GetSingleCohortRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        long accountId = _hashingService.DecodeValue(message.AccountId);

        try
        {
            var cohortsResponse = await _commitmentV2Service.GetCohorts(accountId);
            if (cohortsResponse.Count() != 1) return new GetSingleCohortResponse();

            var singleCohort = cohortsResponse.Single();

            if (singleCohort.NumberOfDraftApprentices > 0)
            {
                singleCohort.Apprenticeships = await _commitmentV2Service.GetDraftApprenticeships(singleCohort);
            }
            return new GetSingleCohortResponse
            {
                Cohort = singleCohort
            };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Failed to get Cohorts for {message.AccountId}");
            return new GetSingleCohortResponse
            {
                HasFailed = true
            };
        }
    }
}