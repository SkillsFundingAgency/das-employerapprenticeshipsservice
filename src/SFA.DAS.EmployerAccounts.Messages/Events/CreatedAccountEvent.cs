using System;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class CreatedAccountEvent
    {
        public long AccountId { get; set; }
        public string HashedId { get; set; }
        public string PublicHashedId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
        public DateTime Created { get; set; }
    }
}
