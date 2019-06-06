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
using Fix = SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Commitments.CohortApprovalRequestedByProviderEventHandlerTestsFixture;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Commitments
{
    [TestFixture]
    [Parallelizable]
    public class CohortApprovalRequestedByProviderEventHandlerTests : FluentTest<CohortApprovalRequestedByProviderEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingCohortApprovalRequestedByProvider_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }
        
        [Test]
        public Task Handle_WhenAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewCohort()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyAccountDocumentSavedWithCohort());
        }
        
        [Test]
        public Task Handle_WhenAccountDoesNotContainOrganisation_ThenAccountDocumentIsSavedWithNewCohort()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(f.Message.AccountId),f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithCohort());
        }

        [Test]
        public Task Handle_WhenAccountDoesContainOrganisationButNotCohort_ThenAccountDocumentIsSavedWithNewCohort()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsOrganisation(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithCohort());
        }

        [Test]
        [Ignore("In progress")]
        public Task Handle_WhenAccountDoesContainOrganisationAndCohort_ThenAccountDocumentIsSavedWithUpdatedCohort()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsCohort(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithCohort());
        }
        
        [Test]
        public Task Execute_WhenProviderCommitmentApiThrows_ThenExceptionIsPropagated()
        {
            return TestExceptionAsync(f => f.ArrangeProviderCommitmentApiThrowsException(), f => f.Handle(), 
                (f, r) => r.Should().Throw<Exception>().WithMessage(Fix.ProviderCommitmentsApiExceptionMessage));
        }
    }

    public class CohortApprovalRequestedByProviderEventHandlerTestsFixture : EventHandlerTestsFixture<
        CohortApprovalRequestedByProvider, CohortApprovalRequestedByProviderEventHandler>
    {
        public Mock<IProviderCommitmentsApi> ProviderCommitmentsApi { get; set; }
        public Mock<IHashingService> HashingService { get; set; }
        public CommitmentView Commitment { get; set; }
        public CommitmentView ExpectedCommitment { get; set; }
        public const long AccountLegalEntityId = 456L;
        public const string ProviderCommitmentsApiExceptionMessage = "Test message";

        public CohortApprovalRequestedByProviderEventHandlerTestsFixture() : base(false)
        {
            Commitment = Fixture.Create<CommitmentView>();
            
            ProviderCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            ProviderCommitmentsApi
                .Setup(m => m.GetProviderCommitment(Message.ProviderId, Message.CommitmentId))
                .ReturnsAsync(Commitment);

            HashingService = new Mock<IHashingService>();
            HashingService
                .Setup(m => m.DecodeValue(Commitment.AccountLegalEntityPublicHashedId))
                .Returns(AccountLegalEntityId);                    
            
            Handler = new CohortApprovalRequestedByProviderEventHandler(
                AccountDocumentService.Object,
                MessageContextInitialisation.Object,
                Logger.Object,
                ProviderCommitmentsApi.Object,
                HashingService.Object);
        }
        
        public CohortApprovalRequestedByProviderEventHandlerTestsFixture ArrangeAccountDocumentContainsOrganisation()
        {
            var organisation = SetUpAccountDocumentWithOrganisation(Message.AccountId, AccountLegalEntityId);
            organisation.Cohorts = new List<Cohort>();
            
            AccountDocumentService.Setup(s => s.Get(Message.AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
            return this;
        }

        public CohortApprovalRequestedByProviderEventHandlerTestsFixture ArrangeProviderCommitmentApiThrowsException()
        {
            ProviderCommitmentsApi.Setup(c => c.GetProviderCommitment(Message.ProviderId, Message.CommitmentId))
                .ThrowsAsync(new Exception(ProviderCommitmentsApiExceptionMessage));
            
            return this;
        }

        public CohortApprovalRequestedByProviderEventHandlerTestsFixture ArrangeAccountDocumentContainsCohort()
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
        
        public CohortApprovalRequestedByProviderEventHandlerTestsFixture VerifyAccountDocumentSavedWithCohort()
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
