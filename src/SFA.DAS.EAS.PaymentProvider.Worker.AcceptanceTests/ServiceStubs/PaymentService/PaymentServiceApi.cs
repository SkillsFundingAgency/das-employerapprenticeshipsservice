using SFA.DAS.ApiSubstitute.WebAPI;

namespace SFA.DAS.EAS.PaymentProvider.Worker.AcceptanceTests.ServiceStubs.PaymentService
{
    public class PaymentServiceApi : WebApiSubstitute
    {
        public PaymentServiceApiMessageHandler ApiMessageHandler { get; }
        public string BaseAddress { get; }

        public PaymentServiceApi(PaymentServiceApiMessageHandler apiMessageHandler) : base(apiMessageHandler)
        {
            ApiMessageHandler = apiMessageHandler;
            BaseAddress = apiMessageHandler.BaseAddress;
        }
    }
}
