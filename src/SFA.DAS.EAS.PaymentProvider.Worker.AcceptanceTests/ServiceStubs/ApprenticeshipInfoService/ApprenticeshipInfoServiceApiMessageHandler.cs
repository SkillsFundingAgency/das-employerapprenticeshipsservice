using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.Apprenticeships.Api.Types;
using System.Collections.Generic;
using System.Net;

namespace SFA.DAS.EAS.PaymentProvider.Worker.AcceptanceTests.ServiceStubs.ApprenticeshipInfoService
{
    public class ApprenticeshipInfoServiceApiMessageHandler : ApiMessageHandlers
    {
        public string StandardCode { get; set; }

        public ApprenticeshipInfoServiceApiMessageHandler(string baseAddress) : base(baseAddress)
        { }

        public void SetStandards(HttpStatusCode statusCode, IEnumerable<StandardSummary> response)
        {
            SetupGet($"/standards", statusCode, response);
        }
    }
}
