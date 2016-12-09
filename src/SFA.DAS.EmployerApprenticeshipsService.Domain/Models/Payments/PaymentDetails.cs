using System;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.Domain.Models.Payments
{
    public class PaymentDetails : PaymentEntry
    {
        public string PeriodEnd { get; set; }
        public string ProviderName { get; set; }
        public string CourseName { get; set; }
        public string ApprenticeName { get; set; }
        public string ApprenticeNINumber { get; set; }
    }
}
