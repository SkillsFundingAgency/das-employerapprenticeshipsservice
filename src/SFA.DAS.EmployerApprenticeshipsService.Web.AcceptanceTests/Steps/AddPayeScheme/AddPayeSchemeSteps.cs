﻿using System;
using System.Linq;
using System.Net;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Web.AcceptanceTests.Steps.AddPayeScheme
{
    [Binding, Explicit]
    public class AddPayeSchemeSteps
    {
        private static IContainer _container;
        private static int _exceptionCount;
        private static int _unauthorizedCount;

        [BeforeFeature]
        public static void Arrange()
        {
            var messagePublisher = new Mock<IMessagePublisher>();
            var owinWrapper = new Mock<IAuthenticationService>();
            var cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            var eventsApi = new Mock<IEventsApi>();

            var commitmentsApi = new Mock<IEmployerCommitmentApi>();
            
            _container = IoC.CreateContainer(messagePublisher, owinWrapper, cookieService, eventsApi, commitmentsApi);
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
            _unauthorizedCount = 0;
        }

        [When(@"I remove a scheme")]
        public void WhenIRemoveAScheme()
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();
            var legalEntities = employerPayeOrchestrator.Get(hashedId, userId).Result;
            var scheme = legalEntities.Data.PayeSchemes.First();

            var removeScheme = new RemovePayeSchemeViewModel
            {
                HashedAccountId = hashedId,
                RemoveScheme = 2,
                UserId = userId,
                AccountName = "",
                PayeRef = scheme.Ref
            };

            try
            {
                var result = employerPayeOrchestrator.RemoveSchemeFromAccount(removeScheme).Result;
                if (result.Status == HttpStatusCode.Unauthorized)
                {
                    _unauthorizedCount++;
                }
            }
            catch(Exception)
            {
                _exceptionCount++;
            }
        }

        [Then(@"Scheme is ""(.*)""")]
        public void ThenSchemeIs(string schemeStatus)
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();

            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();
            var payeSchemes = employerPayeOrchestrator.Get(hashedId, userId).Result;

            if (schemeStatus.Equals("removed", StringComparison.CurrentCultureIgnoreCase))
            {
                Assert.AreEqual(0, _unauthorizedCount);
                Assert.AreEqual(0, payeSchemes.Data.PayeSchemes.Count);
            }
            else
            {
                Assert.AreEqual(1, payeSchemes.Data.PayeSchemes.Count);
                Assert.AreEqual(1, _unauthorizedCount);
            }
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

            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();

            var confirmNewPayeScheme = new ConfirmNewPayeSchemeViewModel
            {
                HashedAccountId = hashedId,
                PayeScheme = $"{Guid.NewGuid().ToString().Substring(0, 3)}/{Guid.NewGuid().ToString().Substring(0, 7)}",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
            };
            try
            {
                var result = employerPayeOrchestrator.AddPayeSchemeToAccount(confirmNewPayeScheme, userId).Result;
                if (result.Status == HttpStatusCode.Unauthorized)
                {
                    _exceptionCount++;
                }
            }
            catch (Exception)
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
