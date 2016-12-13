using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Web.AcceptanceTests.Steps.AddPayeScheme
{
    [Binding, Explicit]
    public class AddPayeSchemeSteps
    {
        private static IContainer _container;
        private static bool _newLegalEntity;
        private static int _exceptionCount;




        [BeforeFeature]
        public static void Arrange()
        {
            var messagePublisher = new Mock<IMessagePublisher>();
            var owinWrapper = new Mock<IOwinWrapper>();
            var cookieService = new Mock<ICookieService>();

            _container = IoC.CreateContainer(messagePublisher, owinWrapper, cookieService);
        }

        [AfterFeature]
        public static void TearDown()
        {
            _container.Dispose();
        }

        [AfterScenario()]
        public void ResetData()
        {
            _exceptionCount = 0;
            _newLegalEntity = false;
        }

        [When(@"I remove a scheme")]
        public void WhenIRemoveAScheme()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"Scheme is ""(.*)""")]
        public void ThenSchemeIs(string schemeStatus)
        {
            ScenarioContext.Current.Pending();
        }


        [Then(@"I can view all of my PAYE schemes")]
        public void ThenICanViewAllOfMyPAYESchemes()
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();
            var legalEntities = employerPayeOrchestrator.Get(hashedId, userId).Result;

            Assert.AreEqual(1, legalEntities.Data.PayeSchemes.Count);
        }
        

        [When(@"I Add a new PAYE scheme")]
        public void WhenIAddANewPAYEScheme()
        {
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            _newLegalEntity = true;
            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();

            var confirmNewPayeScheme = new ConfirmNewPayeScheme
            {
                HashedAccountId = hashedId,
                PayeScheme = $"{Guid.NewGuid().ToString().Substring(0, 3)}/{Guid.NewGuid().ToString().Substring(0, 7)}",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
            };
            try
            {
                employerPayeOrchestrator.AddPayeSchemeToAccount(confirmNewPayeScheme, userId).Wait();
            }
            catch (Exception ex)
            {
                _exceptionCount++;
            }
        }


        [Then(@"The PAYE scheme Is ""(.*)""")]
        public void ThenThePAYESchemeIs(string schemeStatus)
        {
         
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            //Get the PAYE schemes
            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();
            var payeSchemes = employerPayeOrchestrator.Get(hashedId, userId).Result;
            if (schemeStatus.Equals("created", StringComparison.CurrentCultureIgnoreCase))
            {
                Assert.AreEqual(0, _exceptionCount);
                Assert.AreEqual(2, payeSchemes.Data.PayeSchemes.Count);
            }
            else
            {
                Assert.AreEqual(1, payeSchemes.Data.PayeSchemes.Count);
                Assert.AreEqual(1, _exceptionCount);
            }

        }


        
    }
}
