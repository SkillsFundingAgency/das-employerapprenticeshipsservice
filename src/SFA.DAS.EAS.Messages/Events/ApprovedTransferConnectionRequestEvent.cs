﻿using System;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EAS.Messages.Events
{
    public class ApprovedTransferConnectionRequestEvent : Event
    {
        public Guid ApprovedByUserExternalId { get; set; }
        public long ApprovedByUserId { get; set; }
        public string ApprovedByUserName { get; set; }
        public string ReceiverAccountHashedId { get; set; }
        public long ReceiverAccountId { get; set; }
        public string ReceiverAccountName { get; set; }
        public string SenderAccountHashedId { get; set; }
        public long SenderAccountId { get; set; }
        public string SenderAccountName { get; set; }
        public int TransferConnectionRequestId { get; set; }
    }
}
