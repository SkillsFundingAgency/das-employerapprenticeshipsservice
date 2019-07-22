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
using SFA.DAS.EAS.Portal.Application.EventHandlers.Commitments;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.HashingService;
using SFA.DAS.Testing;
using Fix = SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.Commitments.CohortApprovalRequestedByProviderEventHandlerTestsFixture;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.Commitments
{
    [TestFixture, Parallelizable]
    public class CohortApprovalRequestedByProviderEventHandlerTests : FluentTest<CohortApprovalRequestedByProviderEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewCohort()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist(f.Message.AccountId), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithCohort());
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

    public class CohortApprovalRequestedByProviderEventHandlerTestsFixture
    {
        public AccountEventHandlerTestHelper<CohortApprovalRequestedByProvider, CohortApprovalRequestedByProviderEventHandler> Helper { get; set; }

        public Mock<IProviderCommitmentsApi> ProviderCommitmentsApi { get; set; }
        public Mock<IHashingService> HashingService { get; set; }
        public CommitmentView Commitment { get; set; }
        public CommitmentView ExpectedCommitment { get; set; }
        public const long AccountLegalEntityId = 456L;
        public const string ProviderCommitmentsApiExceptionMessage = "Test message";

        public CohortApprovalRequestedByProvider Message
        {
            get => Helper.Message;
            set => Helper.Message = value;
        }

        public CohortApprovalRequestedByProviderEventHandlerTestsFixture()
        {
            Helper = new AccountEventHandlerTestHelper<CohortApprovalRequestedByProvider, CohortApprovalRequestedByProviderEventHandler>(false);

            Commitment = Helper.Fixture.Create<CommitmentView>();
            
            ProviderCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            ProviderCommitmentsApi
                .Setup(m => m.GetProviderCommitment(Helper.Message.ProviderId, Helper.Message.CommitmentId))
                .ReturnsAsync(Commitment);

            HashingService = new Mock<IHashingService>();
            HashingService
                .Setup(m => m.DecodeValue(Commitment.AccountLegalEntityPublicHashedId))
                .Returns(AccountLegalEntityId);                    
            
            Helper.Handler = new CohortApprovalRequestedByProviderEventHandler(
                Helper.AccountDocumentService.Object,
                Helper.Logger.Object,
                ProviderCommitmentsApi.Object,
                HashingService.Object);
        }
        
        public CohortApprovalRequestedByProviderEventHandlerTestsFixture ArrangeAccountDoesNotExist(long accountId)
        {
            Helper.ArrangeAccountDoesNotExist(accountId);

            return this;
        }

        public CohortApprovalRequestedByProviderEventHandlerTestsFixture ArrangeEmptyAccountDocument(long accountId)
        {
            Helper.ArrangeEmptyAccountDocument(accountId);

            return this;
        }

        public CohortApprovalRequestedByProviderEventHandlerTestsFixture ArrangeAccountDocumentContainsOrganisation()
        {
            var organisation = Helper.SetUpAccountDocumentWithOrganisation(
                Helper.Message.AccountId, AccountLegalEntityId);
            organisation.Cohorts = new List<Cohort>();
            
            Helper.AccountDocumentService.Setup(s => 
                s.GetOrCreate(Helper.Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Helper.AccountDocument);
            
            return this;
        }

        public CohortApprovalRequestedByProviderEventHandlerTestsFixture ArrangeProviderCommitmentApiThrowsException()
        {
            ProviderCommitmentsApi.Setup(c => c.GetProviderCommitment(
                    Helper.Message.ProviderId, Helper.Message.CommitmentId))
                .ThrowsAsync(new Exception(ProviderCommitmentsApiExceptionMessage));
            
            return this;
        }

        public CohortApprovalRequestedByProviderEventHandlerTestsFixture ArrangeAccountDocumentContainsCohort()
        {
            var organisation = Helper.SetUpAccountDocumentWithOrganisation(
                Helper.Message.AccountId, AccountLegalEntityId);

            var cohort = organisation.Cohorts.RandomElement();
            cohort.Id = Helper.Message.CommitmentId.ToString();
            cohort.Reference = Commitment.Reference;
            cohort.Apprenticeships = cohort.Apprenticeships.Zip(Commitment.Apprenticeships,
                (accountDocumentApprenticeship, apiApprenticeship) =>
                {
                    accountDocumentApprenticeship.Id = apiApprenticeship.Id;
                    return accountDocumentApprenticeship;
                }).ToList();
            
            Helper.AccountDocumentService.Setup(s => 
                s.GetOrCreate(Helper.Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Helper.AccountDocument);
            
            return this;
        }

        public Task Handle()
        {
            ExpectedCommitment = Commitment.Clone();

            return Helper.Handle();
        }
        
        public void VerifyAccountDocumentSavedWithCohort()
        {
            Helper.AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => AccountIsAsExpected(d)),It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        private bool AccountIsAsExpected(AccountDocument document)
        {
            var expectedAccount = Helper.GetExpectedAccount(Helper.OriginalMessage.AccountId);
            var expectedOrganisation = Helper.GetExpectedOrganisation(
                expectedAccount, AccountLegalEntityId, ExpectedCommitment.LegalEntityName);
            var expectedCohort = GetExpectedCohort(expectedOrganisation);

            expectedCohort.Reference = ExpectedCommitment.Reference;

            bool accountIsAsExpected = Helper.AccountIsAsExpected(expectedAccount, document);

            // this is a workaround for a bug in CompareNetObjects, where it believes equal apprenticeships are not equal
            // so we catch that case with this fluent assertion (and we exclude Apprenticeships from the CompareNetObjects comparison)
            document.Account.Organisations.Should().BeEquivalentTo(expectedAccount.Organisations);
            
            return accountIsAsExpected;
        }
        
        private Cohort GetExpectedCohort(Organisation expectedOrganisation)
        {
            Cohort expectedCohort;
            if (Helper.OriginalAccountDocument == null
                || !Helper.OriginalAccountDocument.Account.Organisations.Any())
            {
                //todo: AddNewCohort()?
                expectedCohort = new Cohort
                {
                    //todo: id should be long
                    Id = Helper.OriginalMessage.CommitmentId.ToString(),
                    Apprenticeships = ExpectedCommitment.Apprenticeships.Select(ea =>
                        new Apprenticeship
                        {
                            Id = ea.Id,
                            FirstName = ea.FirstName,
                            LastName = ea.LastName,
                            CourseName = ea.TrainingName,
                            ProposedCost = ea.Cost,
                            StartDate = ea.StartDate,
                            EndDate = ea.EndDate
                        }).ToList()
                };

                expectedOrganisation.Cohorts.Add(expectedCohort);
                return expectedCohort;
            }
            
            expectedCohort = expectedOrganisation.Cohorts.SingleOrDefault(r => 
                r.Id == Helper.OriginalMessage.CommitmentId.ToString());
            if (expectedCohort == null)
            {
                expectedCohort = new Cohort
                {
                    Id = Helper.OriginalMessage.CommitmentId.ToString(),
                    Apprenticeships = ExpectedCommitment.Apprenticeships.Select(ea =>
                        new Apprenticeship
                        {
                            Id = ea.Id,
                            FirstName = ea.FirstName,
                            LastName = ea.LastName,
                            CourseName = ea.TrainingName,
                            ProposedCost = ea.Cost,
                            StartDate = ea.StartDate,
                            EndDate = ea.EndDate
                        }).ToList()
                };
                expectedOrganisation.Cohorts.Add(expectedCohort);
            }
            else
            {
                foreach (var apiApprenticeship in ExpectedCommitment.Apprenticeships)
                {
                    var expectedApprenticeship = expectedCohort.Apprenticeships.Single(a => a.Id == apiApprenticeship.Id);
                    expectedApprenticeship.FirstName = apiApprenticeship.FirstName;
                    expectedApprenticeship.LastName = apiApprenticeship.LastName;
                    expectedApprenticeship.CourseName = apiApprenticeship.TrainingName;
                    expectedApprenticeship.ProposedCost = apiApprenticeship.Cost;
                    expectedApprenticeship.StartDate = apiApprenticeship.StartDate;
                    expectedApprenticeship.EndDate = apiApprenticeship.EndDate;
                }
            }
            return expectedCohort;
        }
    }
}