using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System;
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
        //public bool ApprenticeshipAdded { get; set; }
        public int? CohortsCount { get; set; }
        public int? ApprenticeshipsCount { get; set; }
        public int? NumberOfDraftApprentices { get; set; }
        public bool HasSingleDraftApprenticeship =>  CohortsCount == 1 && NumberOfDraftApprentices == 1 && ApprenticeshipsCount == 0;
        public string CourseName { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public DateTime? CourseEndDate { get; set; }
        public string ProviderName { get; set; }
        public CohortStatus CohortStatus { get; set; }
        public string HashedDraftApprenticeshipId { get; set; }
        public string HashedCohortReference { get; set; }
        public string ApprenticeName { get; set; }        
        public bool HasSingleReservation => Reservations?.Count == 1 && NumberOfDraftApprentices == 0;
        public string ViewOrEditApprenticeDetails => $"unapproved/{HashedCohortReference}/apprentices/{HashedDraftApprenticeshipId}";
        public string ApprovedOrRejectApprenticeDetails => $"unapproved/{HashedCohortReference}";
    }
}