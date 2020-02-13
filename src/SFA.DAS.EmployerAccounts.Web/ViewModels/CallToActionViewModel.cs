using SFA.DAS.EmployerAccounts.Models.Reservations;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class CallToActionViewModel
    {
        public bool AgreementsToSign { get; set; }
        public List<Reservation> Reservations { get; set; }
        public bool HasReservations => ReservationsCount > 0;
        public int ReservationsCount => Reservations?.Count ?? 0;
        public int PendingReservationsCount => Reservations?.Count(x => x.Status == ReservationStatus.Pending) ?? 0;
        public bool ApprenticeshipAdded { get; set; }
    }
}