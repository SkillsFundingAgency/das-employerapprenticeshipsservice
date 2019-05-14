using System;

namespace SFA.DAS.EAS.Portal.Types
{
    public class ReserveFunding
    {
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ReservationId { get; set; }
    }
}