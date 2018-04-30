﻿using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeAddedMessage : AccountMessageBase
    {
        //We have protected setters to support json serialsation due to the empty constructor
        public string PayeScheme { get; protected set; }

        public PayeSchemeAddedMessage()
        { }

        public PayeSchemeAddedMessage(string payeScheme, long accountId, string creatorName, Guid creatorExternalId) 
            : base(accountId, creatorName, creatorExternalId)
        {
            PayeScheme = payeScheme;
        }
    }
}