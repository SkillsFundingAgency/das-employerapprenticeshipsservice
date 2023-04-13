using System.Threading;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Queries.GetReservations;

public class GetReservationsRequestHandler : IRequestHandler<GetReservationsRequest, GetReservationsResponse>
{
    private readonly IValidator<GetReservationsRequest> _validator;
    private readonly ILogger<GetReservationsRequestHandler> _logger;
    private readonly IReservationsService _service;

    public GetReservationsRequestHandler(
        IValidator<GetReservationsRequest> validator,
        ILogger<GetReservationsRequestHandler> logger,
        IReservationsService service)
    {
        _validator = validator;
        _logger = logger;
        _service = service;
    }

    public async Task<GetReservationsResponse> Handle(GetReservationsRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.LogInformation("Getting reservations for hashed account id {AccountId}", message.AccountId);

        try
        {
            return new GetReservationsResponse
            {
                Reservations = await _service.Get(message.AccountId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Reservations for {AccountId}", message.AccountId);
            return new GetReservationsResponse
            {
                HasFailed = true
            };
        }
    }
}