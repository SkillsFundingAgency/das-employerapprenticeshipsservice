﻿using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("user_invited")]
    public class UserInvitedMessage : AccountMessageBase
    {
        //We have protected setters to support json serialsation due to the empty constructor
        public string PersonInvited { get; protected set; }
        
        public UserInvitedMessage()
        { }

        public UserInvitedMessage(string personInvited, long accountId, string creatorName, Guid creatorUserRef) 
            : base(accountId, creatorName, creatorUserRef)
        {
            PersonInvited = personInvited;
        }
    }
}
