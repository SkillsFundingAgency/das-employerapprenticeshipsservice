using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.Provider.Events.Api.Types;
using System.Collections.Generic;
using System.Net;
using Payment = SFA.DAS.Provider.Events.Api.Types.Payment;
using PeriodEnd = SFA.DAS.Provider.Events.Api.Types.PeriodEnd;

namespace SFA.DAS.EAS.PaymentProvider.Worker.AcceptanceTests.ServiceStubs.PaymentService
{
    public class PaymentServiceApiMessageHandler : ApiMessageHandlers
    {
        public int PageNumber { get; set; }
        public string PeriodEnd { get; set; }
        public long EmployerAccountId { get; set; }
        public long? UkPrn { get; set; }

        public PaymentServiceApiMessageHandler(string baseAddress) : base(baseAddress)
        {
            PageNumber = 1;
        }

        public void SetPeriodEnds(HttpStatusCode statusCode, IEnumerable<PeriodEnd> response)
        {
            SetupGet($"/api/periodends", statusCode, response);
        }

        public void SetPayments(HttpStatusCode statusCode, PageOfResults<Payment> response)
        {
            SetupGet(
                 $"/api/payments?page={PageNumber}" +
                 $"&periodId={PeriodEnd}&employerAccountId={EmployerAccountId}&ukprn={UkPrn}",
                 statusCode,
                 response);
        }

        public void SetTransfers(HttpStatusCode statusCode, PageOfResults<Transfer> response)
        {
            SetupGet($"/api/accountTransfers/{PeriodEnd}/{EmployerAccountId}", statusCode, response);
        }
    }
}
