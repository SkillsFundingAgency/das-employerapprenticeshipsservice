using SFA.DAS.EmployerAccounts.Models.Reservations;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class CallToActionViewModel
{
    public List<Reservation> Reservations { get; set; }
    public bool HasReservations => ReservationsCount > 0;
    public int ReservationsCount => Reservations?.Count ?? 0;
    public int PendingReservationsCount => Reservations?.Count(x => x.Status == ReservationStatus.Pending) ?? 0;

    public IEnumerable<ApprenticeshipViewModel> Apprenticeships { get; set; }
    public int ApprenticeshipsCount => Apprenticeships?.Count() ?? 0;

    public IEnumerable<CohortViewModel> Cohorts { get; set; }
    public int CohortsCount => Cohorts?.Count() ?? 0;        
    public bool HasSingleReservation => Reservations?.Count == 1 && CohortsCount == 1;
    public VacanciesViewModel VacanciesViewModel { get; set; }
        
    public CallToActionViewModel()
    {
        VacanciesViewModel = new VacanciesViewModel();
    }
}