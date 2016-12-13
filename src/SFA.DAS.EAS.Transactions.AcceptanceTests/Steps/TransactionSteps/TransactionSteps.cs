using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.TransactionSteps
{
    [Binding, Explicit]
    public class TransactionSteps
    {
        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;
        private string _hashedAccountId;
        private static Mock<ICookieService> _cookieService;


        [BeforeFeature]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieService>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService);

        }

        [AfterFeature]
        public static void TearDown()
        {
            _container.Dispose();
        }


        [When(@"I have the following submissions")]
        public void WhenIHaveTheFollowingSubmissions(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I have the following payments")]
        public void WhenIHaveTheFollowingPayments(Table table)
        {
            ScenarioContext.Current.Pending();
        }


        [Then(@"the balance should be (.*) on the screen")]
        public void ThenTheBalanceShouldBeOnTheScreen(int balance)
        {
            ScenarioContext.Current.Pending();
        }


    }
}
