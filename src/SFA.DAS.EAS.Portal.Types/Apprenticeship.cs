using System;

namespace SFA.DAS.EAS.Portal.Types
{
    public class Apprenticeship
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? ProposedCost { get; set; }
        public Provider TrainingProvider { get; set; }
    }
}