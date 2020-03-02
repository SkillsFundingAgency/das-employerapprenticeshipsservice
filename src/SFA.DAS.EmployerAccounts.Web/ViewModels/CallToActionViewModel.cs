using SFA.DAS.EmployerAccounts.Models.Commitments;
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

        public CohortsV2ViewModel CohortsV2ViewModel { get; set; }
        public int CohortsV2ViewModelCount => CohortsV2ViewModel?.CohortV2WebViewModel?.Count() ?? 0;        
        public bool HasSingleReservation => Reservations?.Count == 1 && CohortsV2ViewModel.CohortV2WebViewModel?.First().NumberOfDraftApprentices == 0;
       
    }
}