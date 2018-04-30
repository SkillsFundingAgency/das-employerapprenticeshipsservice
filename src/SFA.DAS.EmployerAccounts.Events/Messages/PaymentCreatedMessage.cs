using SFA.DAS.Messaging.Attributes;
using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("payment_created")]
    public class PaymentCreatedMessage : AccountMessageBase
    {
        public decimal Amount { get; protected set; }
        public string ProviderName { get; protected set; }

        public PaymentCreatedMessage() 
            : base(0, string.Empty, Guid.Empty)
        {}

        public PaymentCreatedMessage(
            string providerName, decimal amount, long accountId, string creatorName, Guid externalUserId) 
            : base(accountId, creatorName, externalUserId)
        {
            ProviderName = providerName;
            Amount = amount;
        }
    }
}
