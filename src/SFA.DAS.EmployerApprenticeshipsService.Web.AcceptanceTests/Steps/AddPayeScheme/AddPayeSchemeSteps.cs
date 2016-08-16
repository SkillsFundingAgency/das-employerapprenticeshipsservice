using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DbCleanup;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DependencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.AddPayeScheme
{
    [Binding, Explicit]
    public class AddPayeSchemeSteps
    {
        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static ICleanDatabase _cleanDownDb;
        private static Mock<IOwinWrapper> _owinWrapper;
        private static bool _newLegalEntity;
        private static int _exceptionCount;

        [BeforeFeature()]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper);

            _cleanDownDb = _container.GetInstance<ICleanDatabase>();
            _cleanDownDb.Execute().Wait();
        }

        [AfterFeature()]
        public static void TearDown()
        {
            _exceptionCount = 0;
            _newLegalEntity = false;

            _cleanDownDb.Execute().Wait();

            _container.Dispose();
        }

        [AfterScenario()]
        public void ResetData()
        {
            _exceptionCount = 0;
            _newLegalEntity = false;
        }
        
        [Then(@"I can view all of my PAYE schemes")]
        public void ThenICanViewAllOfMyPAYESchemes()
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();
            var legalEntities = employerPayeOrchestrator.Get(accountId, userId).Result;

            Assert.AreEqual(1, legalEntities.Data.PayeSchemes.Count);
        }

        [When(@"I Add a new PAYE scheme to my existing legal entity")]
        public void WhenIAddANewPAYESchemeToMyExistingLegalEntity()
        {
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();
            var accountId = (long)ScenarioContext.Current["AccountId"];
            _newLegalEntity = false;

            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();

            var legalEntity = employerPayeOrchestrator.Get(accountId, userId).Result.Data.PayeSchemes.FirstOrDefault();

            var confirmNewPayeScheme = new ConfirmNewPayeScheme
            {
                AccountId = accountId,
                LegalEntityId = legalEntity.LegalEntityId,
                PayeScheme = $"{Guid.NewGuid().ToString().Substring(0, 3)}/{Guid.NewGuid().ToString().Substring(0, 7)}",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString()
            };
            try
            {
                employerPayeOrchestrator.AddPayeSchemeToAccount(confirmNewPayeScheme, userId).Wait();
            }
            catch (Exception)
            {
                _exceptionCount++;
            }
        }

        [When(@"I Add a new PAYE scheme to my new legal entity")]
        public void WhenIAddANewPAYESchemeToMyNewLegalEntity()
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            _newLegalEntity = true;
            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();

            var confirmNewPayeScheme = new ConfirmNewPayeScheme
            {
                AccountId = accountId,
                LegalEntityId = 0,
                PayeScheme = $"{Guid.NewGuid().ToString().Substring(0, 3)}/{Guid.NewGuid().ToString().Substring(0, 7)}",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                LegalEntityCode = "12345",
                LegalEntityDateOfIncorporation = new DateTime(2016,10,30),
                LegalEntityName = "Test Entity",
                LegalEntityRegisteredAddress = "Test Address"
            };
            try
            {
                employerPayeOrchestrator.AddPayeSchemeToAccount(confirmNewPayeScheme, userId).Wait();
            }
            catch (Exception)
            {
                _exceptionCount ++;
            }
            
        }


        [Then(@"The PAYE scheme Is ""(.*)""")]
        public void ThenThePAYESchemeIs(string schemeStatus)
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            //Get the PAYE schemes
            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();
            var legalEntities = employerPayeOrchestrator.Get(accountId, userId).Result;
            var entities = legalEntities.Data.PayeSchemes.Select(c => c.LegalEntityId).Distinct().Count();
            if (schemeStatus.Equals("created", StringComparison.CurrentCultureIgnoreCase))
            {
                Assert.AreEqual(_newLegalEntity ? 2 : 1, entities);
                Assert.AreEqual(2, legalEntities.Data.PayeSchemes.Count);
                Assert.AreEqual(0, _exceptionCount);
            }
            else
            {
                Assert.AreEqual(1, entities);
                Assert.AreEqual(1, legalEntities.Data.PayeSchemes.Count);
                Assert.AreEqual(1, _exceptionCount);
            }

        }


        
    }
}
