using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.Reservations;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.Reservations;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using ReservationStatus = SFA.DAS.EmployerAccounts.Models.Reservations.ReservationStatus;

namespace SFA.DAS.EmployerAccounts.Services;

public class ReservationsService : IReservationsService
{
    private readonly IOuterApiClient _outerApiClient;
    private readonly ILogger<ReservationsService> _logger;
    public ReservationsService(IOuterApiClient apiClient, ILogger<ReservationsService> logger)
    {
        _logger = logger;
        _outerApiClient = apiClient;
    }

    public async Task<IEnumerable<Reservation>> Get(long accountId)
    {
        IEnumerable<Reservation> reservation = null;

        try
        {
            _logger.LogInformation("Getting reservations for account ID: {AccountId}", accountId);

            var reservationsResponse = await _outerApiClient.Get<GetReservationsResponse>(new GetReservationsRequest(accountId));

            if (reservationsResponse != null)
            {
                reservation =  MapFrom(reservationsResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not find reservations for account ID: {AccountId} when calling reservations API", accountId);
        }

        return reservation;
    }

    private static IEnumerable<Reservation> MapFrom(GetReservationsResponse getReservationsResponse)
    {
        IEnumerable<Reservation> reservation = getReservationsResponse.Reservations.Select(x => new Reservation
        {
            Id = x.Id,
            AccountId = x.AccountId,
            CreatedDate = x.CreatedDate,
            StartDate = x.StartDate,
            ExpiryDate = x.ExpiryDate,
            Course = new Course
            {
                CourseId = x.Course.CourseId,
                Title = x.Course.Title,
                Level = x.Course.Level
            },
            Status = (ReservationStatus)x.Status,
            AccountLegalEntityId = x.AccountLegalEntityId,
            AccountLegalEntityName = x.AccountLegalEntityName
        }).ToList();

        return reservation;
    }
}