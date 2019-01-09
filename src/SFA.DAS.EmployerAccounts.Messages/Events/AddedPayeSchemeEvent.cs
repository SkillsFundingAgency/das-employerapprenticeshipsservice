using System;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class AddedPayeSchemeEvent 
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
        public string PayeRef { get; set; }
        public DateTime Created { get; set; }
    }
}
