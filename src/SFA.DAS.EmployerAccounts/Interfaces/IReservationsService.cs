using SFA.DAS.EmployerAccounts.Models.Reservations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IReservationsService
    {
        Task<IEnumerable<Reservation>> Get(long accountId);
    }
}
