using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Models.Reservations;

namespace SFA.DAS.EmployerAccounts.Services;

public class ReservationsServiceWithTimeout : IReservationsService
{
    private readonly IReservationsService _service;
    private readonly IAsyncPolicy _pollyPolicy;

    public ReservationsServiceWithTimeout(IReservationsService service, IReadOnlyPolicyRegistry<string> pollyRegistry)
    {
        _service = service;
        _pollyPolicy = pollyRegistry.Get<IAsyncPolicy>(Constants.DefaultServiceTimeout);
    }

    public async Task<IEnumerable<Reservation>> Get(long accountId)
    {
        try
        {
            return await _pollyPolicy.ExecuteAsync(() => _service.Get(accountId));
        }
        catch (TimeoutRejectedException ex)
        {
            throw new ServiceTimeoutException("Call to Reservation Service timed out", ex);
        }
    }
}