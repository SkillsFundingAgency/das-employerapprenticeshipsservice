using System;

namespace SFA.DAS.EmployerFinance.Models.UserProfile
{
    public class User
    {
        public virtual long Id { get; set; }

        public virtual Guid Ref { get; set; }
        public virtual string Email { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string CorrelationId { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
