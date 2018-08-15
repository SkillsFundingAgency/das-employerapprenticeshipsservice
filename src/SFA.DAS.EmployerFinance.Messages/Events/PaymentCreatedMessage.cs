using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerFinance.Messages.Events
{
    [Serializable]
    [MessageGroup("payment_created")]
    public class PaymentCreatedMessage : PaymentMessageBase
    {
        public decimal Amount { get; protected set; }
        public string ProviderName { get; protected set; }

        public PaymentCreatedMessage() 
            : base(0, string.Empty, string.Empty)
        {}

        public PaymentCreatedMessage(
            string providerName, decimal amount, long accountId, string creatorName, string creatorUserRef) 
            : base(accountId, creatorName, creatorUserRef)
        {
            ProviderName = providerName;
            Amount = amount;
        }
    }
}
