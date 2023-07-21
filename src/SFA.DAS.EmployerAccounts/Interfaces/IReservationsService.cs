using SFA.DAS.EmployerAccounts.Models.Reservations;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IReservationsService
{
    Task<IEnumerable<Reservation>> Get(long accountId);
}