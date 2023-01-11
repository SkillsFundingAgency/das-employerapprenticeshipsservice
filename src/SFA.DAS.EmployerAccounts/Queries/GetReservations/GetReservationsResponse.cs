using SFA.DAS.EmployerAccounts.Models.Reservations;

namespace SFA.DAS.EmployerAccounts.Queries.GetReservations;

public class GetReservationsResponse 
{
    public IEnumerable<Reservation> Reservations { get; set; }
    public bool HasFailed { get; set; }
}