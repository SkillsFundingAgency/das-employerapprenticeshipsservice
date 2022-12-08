using System;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class InvitedUserEvent
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
        public string PersonInvited { get; set; }
        public DateTime Created { get; set; }
    }
}
