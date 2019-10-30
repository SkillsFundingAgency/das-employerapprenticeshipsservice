using System;

namespace SFA.DAS.EmployerAccounts.Models.Reservations
{    
    public class Reservation
    {
        public Guid Id { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public long AccountId { get; set; }
        public Course Course { get; set; }
    }
}
