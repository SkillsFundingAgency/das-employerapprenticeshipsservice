using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("user_invited")]
    public class UserInvitedMessage : AccountMessageBase
    {
        public string PersonInvited { get; }
        
        public UserInvitedMessage()
        { }

        public UserInvitedMessage(string personInvited, long accountId, string creatorName, string creatorUserRef) 
            : base(accountId, creatorName, creatorUserRef)
        {
            PersonInvited = personInvited;
        }
    }
}
