using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class ReserveFundingCreatedEvent : Event
    {
        public string HashedAccountId { get; set; }
        public string CourseCode { get; set; }
        public string ApprenticeName { get; set; }
        public string CourseName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long ReservationId { get; set; }
    }
}
