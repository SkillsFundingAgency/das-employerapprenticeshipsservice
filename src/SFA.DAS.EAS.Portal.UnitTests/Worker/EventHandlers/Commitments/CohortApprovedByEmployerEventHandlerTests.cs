using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments;
using SFA.DAS.HashingService;
using SFA.DAS.Testing;
using Fix = SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Commitments.CohortApprovedByEmployerEventHandlerTestsFixture;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Commitments
{
    [TestFixture]
    [Parallelizable]
    public class CohortApprovedByEmployerEventHandlerTests : FluentTest<CohortApprovedByEmployerEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingCohortApprovalRequestedByProvider_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }

        [Test]
        [Ignore("In progress")]
        public Task Handle_WhenAccountDoesContainOrganisationAndCohort_ThenAccountDocumentIsSavedWithUpdatedCohort()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsCohort(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithCohort());
        }
    }

    public class CohortApprovedByEmployerEventHandlerTestsFixture : EventHandlerTestsFixture<
        CohortApprovedByEmployer, CohortApprovedByEmployerEventHandler>
    {
        public Mock<IProviderCommitmentsApi> ProviderCommitmentsApi { get; set; }
        public Mock<IHashingService> HashingService { get; set; }
        public CommitmentView Commitment { get; set; }
        public CommitmentView ExpectedCommitment { get; set; }
        public const long AccountLegalEntityId = 456L;
        public const string ProviderCommitmentsApiExceptionMessage = "Test message";

        public CohortApprovedByEmployerEventHandlerTestsFixture() : base(false)
        {

            Handler = new CohortApprovedByEmployerEventHandler(
                AccountDocumentService.Object,
                MessageContextInitialisation.Object,
                Logger.Object);
        }

        public CohortApprovedByEmployerEventHandlerTestsFixture ArrangeAccountDocumentContainsCohort()
        {
            var organisation = SetUpAccountDocumentWithOrganisation(Message.AccountId, AccountLegalEntityId);

            organisation.Cohorts.RandomElement().Id = Message.CommitmentId.ToString();
            
            AccountDocumentService.Setup(s => s.Get(Message.AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
            return this;
        }

        public override Task Handle()
        {
            ExpectedCommitment = Commitment.Clone();

            return base.Handle();
        }
        
        public CohortApprovedByEmployerEventHandlerTestsFixture VerifyAccountDocumentSavedWithCohort()
        {
            AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => AccountIsAsExpected(d)),It.IsAny<CancellationToken>()), Times.Once);
            
            return this;
        }
        
        private bool AccountIsAsExpected(AccountDocument document)
        {
            var expectedAccount = GetExpectedAccount(OriginalMessage.AccountId);
            var expectedOrganisation = GetExpectedOrganisation(
                expectedAccount, AccountLegalEntityId, ExpectedCommitment.LegalEntityName);
            var expectedCohort = GetExpectedCohort(expectedOrganisation);

            expectedCohort.Reference = ExpectedCommitment.Reference;
            expectedCohort.Apprenticeships = ExpectedCommitment.Apprenticeships.Select(ea =>
                new Apprenticeship
                {
                    Id = ea.Id,
                    FirstName = ea.FirstName,
                    LastName = ea.LastName,
                    CourseName = ea.TrainingName,
                    ProposedCost = ea.Cost,
                    StartDate = ea.StartDate,
                    EndDate = ea.EndDate
                }).ToList();

            return AccountIsAsExpected(expectedAccount, document);
        }
        
        private Cohort GetExpectedCohort(Organisation expectedOrganisation)
        {
            Cohort expectedCohort;
            if (OriginalAccountDocument == null
                || !OriginalAccountDocument.Account.Organisations.Any())
            {
                //todo: AddNewCohort()?
                expectedCohort = new Cohort
                {
                    //todo: id should be long
                    Id = OriginalMessage.CommitmentId.ToString()
                };

                expectedOrganisation.Cohorts.Add(expectedCohort);
                return expectedCohort;
            }
            
            expectedCohort = expectedOrganisation.Cohorts.SingleOrDefault(r => r.Id == OriginalMessage.CommitmentId.ToString());
            if (expectedCohort == null)
            {
                expectedCohort = new Cohort
                {
                    Id = OriginalMessage.CommitmentId.ToString()
                };
                expectedOrganisation.Cohorts.Add(expectedCohort);
            }
            return expectedCohort;
        }
    }
}
