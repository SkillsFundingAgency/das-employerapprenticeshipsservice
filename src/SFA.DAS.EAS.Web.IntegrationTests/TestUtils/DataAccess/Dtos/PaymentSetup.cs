using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos
{
    public class PaymentSetup
    {
        public PaymentDetails PaymentInput { get; set; }
        public Payment PaymentOutput { get; set; }
    }
}