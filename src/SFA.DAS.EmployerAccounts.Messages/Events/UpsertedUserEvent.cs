
using System;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class UpsertedUserEvent 
    {
        public string UserRef { get; set; }

        public string CorrelationId { get; set; }
        public DateTime Created { get; set; }
    }
}
