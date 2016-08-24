using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.EmployerApprenticeshipsService.TestCommon.DbCleanup;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DependencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.CommonSteps
{
    [Binding]
    public class GlobalTestSteps
    {
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;
        private static Container _container;

        [AfterTestRun()]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper);

            var cleanDownDb = _container.GetInstance<ICleanDatabase>();
            cleanDownDb.Execute().Wait();
        }
    }
}
