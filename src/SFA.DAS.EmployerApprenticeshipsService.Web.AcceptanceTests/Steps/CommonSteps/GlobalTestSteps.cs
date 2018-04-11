using Moq;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.TestCommon.DbCleanup;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Web.AcceptanceTests.Steps.CommonSteps
{
    [Binding]
    public class GlobalTestSteps
    {
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IAuthenticationService> _owinWrapper;
        private static Container _container;
        private static Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private static Mock<IEventsApi> _eventsApi;
        private static Mock<IEmployerCommitmentApi> _commitmentsApi;

        [AfterTestRun()]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _eventsApi = new Mock<IEventsApi>();
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi, _commitmentsApi);

            var cleanDownDb = _container.GetInstance<ICleanDatabase>();
            cleanDownDb.Execute().Wait();
        }
    }
}
