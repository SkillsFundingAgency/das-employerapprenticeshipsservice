using System;

namespace SFA.DAS.EAS.Portal.Events.Reservations
{
    public class ReserveFundingAddedEvent
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long ReservationId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
