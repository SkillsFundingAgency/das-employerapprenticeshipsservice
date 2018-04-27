using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Commitment;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Web.AcceptanceTests.Steps.RemoveLegalEntity
{
    [Binding]
    public class RemoveLegalEntitySteps
    {
        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IAuthenticationService> _owinWrapper;
        private static Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private static Mock<IEventsApi> _eventsApi;
        private Mock<IEmployerCommitmentApi> _commitmentsApi;
        
        [BeforeScenario()]
        public void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _eventsApi = new Mock<IEventsApi>();
            _commitmentsApi= new Mock<IEmployerCommitmentApi>();
            _commitmentsApi.Setup(x => x.GetEmployerAccountSummary(It.IsAny<long>()))
                .ReturnsAsync(new List<ApprenticeshipStatusSummary>
                {
                    new ApprenticeshipStatusSummary {}
                });

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi, _commitmentsApi);

            var commitmentService = new Mock<ICommitmentService>();
            commitmentService.Setup(x => x.GetEmployerCommitments(It.IsAny<long>())).ReturnsAsync(new List<Cohort>());
            _container.Inject(commitmentService.Object);
        }

        [When(@"I have more than one legal entity with a ""(.*)"" status and an active commitment")]
        public void WhenIHaveMoreThanOneLegalEntityWithAStatusAndAnActiveCommitment(string agreementStatus)
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            _commitmentsApi.Setup(x => x.GetEmployerAccountSummary(accountId))
                .ReturnsAsync(new List<ApprenticeshipStatusSummary>
                {
                    new ApprenticeshipStatusSummary
                    {
                        ActiveCount = 1, LegalEntityIdentifier = "test"
                    },
                    new ApprenticeshipStatusSummary
                    {
                        ActiveCount = 1,LegalEntityIdentifier = ScenarioContext.Current["LegalEntityCode"].ToString()
                    }
                });
            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi, _commitmentsApi);

            var commitmentService = new Mock<ICommitmentService>();
            commitmentService.Setup(x => x.GetEmployerCommitments(It.IsAny<long>())).ReturnsAsync(new List<Cohort>());
            _container.Inject(commitmentService.Object);

            CreateLegalEntityWithStatus(agreementStatus,"test");
        }
        
        [When(@"I have more than one legal entity with a ""(.*)"" status")]
        public void WhenIHaveMoreThanOneLegalEntityWithAStatus(string agreementStatus)
        {
            CreateLegalEntityWithStatus(agreementStatus, "test");
        }

        private static void CreateLegalEntityWithStatus(string agreementStatus, string legalEntityCode)
        {
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["AccountOwnerUserRef"].ToString();
            ScenarioContext.Current["ExpectBadRequestResult"] = "false";

            var organisationOrchestrator = _container.GetInstance<OrganisationOrchestrator>();

            organisationOrchestrator.CreateLegalEntity(new CreateNewLegalEntityViewModel
            {
                HashedAccountId = hashedId,
                Name = "second company",
                ExternalUserId = userId,
                Code = legalEntityCode
            }).Wait();

            if (agreementStatus.ToLower().Equals("signed"))
            {
                var employerAgreementOrchestrator = _container.GetInstance<EmployerAgreementOrchestrator>();

                var agreementsToRemove =
                    employerAgreementOrchestrator.GetLegalAgreementsToRemove(hashedId, userId).Result;

                foreach (var agreement in agreementsToRemove.Data.Agreements)
                {
                    employerAgreementOrchestrator.SignAgreement(agreement.HashedAgreementId, agreement.HashedAccountId,
                        userId, DateTime.UtcNow, "company name").Wait();
                }
                ScenarioContext.Current["ExpectBadRequestResult"] = "true";
            }
        }

        [When(@"There is only one legal entity on the account")]
        public void WhenThereIsOnlyOneLegalEntityOnTheAccount()
        {
            ScenarioContext.Current["ExpectBadRequestResult"] = "true";
        }



        [Then(@"I ""(.*)"" remove a legal entity")]
        public void ThenIRemoveALegalEntity(string canRemove)
        {
            var hashedId = ScenarioContext.Current["HashedAccountId"].ToString();
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();
            var ownerId = ScenarioContext.Current["AccountOwnerUserRef"].ToString();
            var expectBadRequestResult = ScenarioContext.Current["ExpectBadRequestResult"].ToString();

            var employerAgreementOrchestrator = _container.GetInstance<EmployerAgreementOrchestrator>();
            
            var agreementsToRemove = employerAgreementOrchestrator.GetLegalAgreementsToRemove(hashedId, ownerId).Result;

            var agreement = agreementsToRemove.Data.Agreements.FirstOrDefault();

            var actual = employerAgreementOrchestrator.RemoveLegalAgreement(new ConfirmLegalAgreementToRemoveViewModel
            {
                HashedAccountId = agreement.HashedAccountId,
                HashedAgreementId = agreement.HashedAgreementId,
                RemoveOrganisation = 2
            }, userId).Result;

            if (canRemove.ToLower().Equals("can"))
            {
                Assert.AreEqual(HttpStatusCode.OK, actual.Status);
            }
            else
            {
                if (expectBadRequestResult.ToLower().Equals("true"))
                {
                    Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
                }
                else
                {
                    Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
                }
                
            }

            

        }

    }
}
