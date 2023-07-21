using SFA.DAS.EmployerAccounts.Models.Reservations;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class ReservationViewModel
{
    public ReservationViewModel(Reservation reservation)
    {
        Id = reservation?.Id ?? Guid.Empty;
        TrainingDate = new TrainingDateModel
        {
            StartDate = reservation?.StartDate.Value ?? default(DateTime),
            EndDate = reservation?.ExpiryDate.Value ?? default(DateTime)
        };
        CourseDescription = reservation?.Course?.CourseDescription ?? "Unknown";
    }

    public Guid Id { get; private set; }
    public TrainingDateModel TrainingDate { get; private set; }
    public object CourseDescription { get; private set; }
}