using System;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Web.Models;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class ReservationViewModel
    {
        //public ReservationViewModel(Reservation reservation)
        //{
        //    Id = reservation?.Id ?? Guid.Empty;
        //    TrainingDate = new TrainingDateModel
        //    {
        //        StartDate = reservation?.StartDate.Value ?? default(DateTime),
        //        EndDate = reservation?.ExpiryDate.Value ?? default(DateTime)
        //    };
        //    CourseDescription = reservation?.Course?.CourseDescription ?? "Unknown";
        //}

        public Guid Id { get; set; }
        public TrainingDateModel TrainingDate { get; set; }
        public object CourseDescription { get; set; }
    }
}