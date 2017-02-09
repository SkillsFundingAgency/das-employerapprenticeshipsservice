﻿using Moq;
using SFA.DAS.EAS.TestCommon.DbCleanup;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Web.AcceptanceTests.Steps.CommonSteps
{
    [Binding]
    public class GlobalTestSteps
    {
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;
        private static Container _container;
        private static Mock<ICookieService> _cookieService;

        [AfterTestRun()]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieService>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService);

            var cleanDownDb = _container.GetInstance<ICleanDatabase>();
            cleanDownDb.Execute().Wait();
        }
    }
}
