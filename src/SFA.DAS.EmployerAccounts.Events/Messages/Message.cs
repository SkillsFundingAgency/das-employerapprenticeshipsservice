using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public abstract class Message
    {
        protected Message()
        {
            PostedDatedTime = DateTime.Now;
        }

        public DateTime PostedDatedTime { get;  }
    }
}
