﻿using SFA.DAS.Messaging.Attributes;
using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("paymentdataimport_completed")]
    public class RefreshPaymentDataCompletedMessage : AccountMessageBase
    {
        public string PayrollPeriod { get; set; }

        public bool PaymentsProcessed { get; set; }

        public RefreshPaymentDataCompletedMessage()
            : base(0, string.Empty, string.Empty)
        {
        }

        public RefreshPaymentDataCompletedMessage(
            long accountId, bool paymentsProcessed, DateTime timestamp, string payrollPeriod, string creatorName, string creatorUserRef)
            : base(accountId, creatorName, creatorUserRef)
        {
            CreatedAt = timestamp;
            PaymentsProcessed = paymentsProcessed;
            PayrollPeriod = payrollPeriod;
        }
    }
}