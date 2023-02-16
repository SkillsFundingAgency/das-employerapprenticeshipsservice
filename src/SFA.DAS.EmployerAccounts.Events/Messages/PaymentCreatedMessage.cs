using SFA.DAS.Messaging.Attributes;
using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

[Serializable]
[MessageGroup("payment_created")]
[Obsolete("Please use CreatedPaymentEvent from EmployerFinance")]
public class PaymentCreatedMessage : AccountMessageBase
{
    public decimal Amount { get; protected set; }
    public string ProviderName { get; protected set; }

    public PaymentCreatedMessage()
        : base(0, string.Empty, string.Empty)
    { }

    public PaymentCreatedMessage(
        string providerName, decimal amount, long accountId, string creatorName, string creatorUserRef)
        : base(accountId, creatorName, creatorUserRef)
    {
        ProviderName = providerName;
        Amount = amount;
    }
}