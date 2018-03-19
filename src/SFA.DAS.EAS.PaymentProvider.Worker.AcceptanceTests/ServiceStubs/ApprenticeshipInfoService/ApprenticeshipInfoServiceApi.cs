using SFA.DAS.ApiSubstitute.WebAPI;

namespace SFA.DAS.EAS.PaymentProvider.Worker.AcceptanceTests.ServiceStubs.ApprenticeshipInfoService
{
    public class ApprenticeshipInfoServiceApi : WebApiSubstitute
    {
        public ApprenticeshipInfoServiceApiMessageHandler ApiMessageHandler { get; }
        public string BaseAddress { get; }

        public ApprenticeshipInfoServiceApi(ApprenticeshipInfoServiceApiMessageHandler apiMessageHandler) : base(apiMessageHandler)
        {
            ApiMessageHandler = apiMessageHandler;
            BaseAddress = apiMessageHandler.BaseAddress;
        }


    }
}
