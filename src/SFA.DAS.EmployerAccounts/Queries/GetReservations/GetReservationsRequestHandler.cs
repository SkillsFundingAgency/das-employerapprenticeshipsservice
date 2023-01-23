using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetReservations;

public class GetReservationsRequestHandler : IRequestHandler<GetReservationsRequest, GetReservationsResponse>
{
    private readonly IValidator<GetReservationsRequest> _validator;
    private readonly ILogger<GetReservationsRequestHandler> _logger;
    private readonly IReservationsService _service;
    private readonly IHashingService _hashingService;

    public GetReservationsRequestHandler(
        IValidator<GetReservationsRequest> validator,
        ILogger<GetReservationsRequestHandler> logger,
        IReservationsService service,
        IHashingService hashingService)
    {
        _validator = validator;
        _logger = logger;
        _service = service;
        _hashingService = hashingService;
    }

    public async Task<GetReservationsResponse> Handle(GetReservationsRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        long accountId = _hashingService.DecodeValue(message.HashedAccountId);

        _logger.LogInformation($"Getting reservations for hashed account id {message.HashedAccountId}");

        try
        {
            return new GetReservationsResponse
            {
                Reservations = await _service.Get(accountId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get Reservations for {message.HashedAccountId}");
            return new GetReservationsResponse
            {
                HasFailed = true
            };
        }
    }
}