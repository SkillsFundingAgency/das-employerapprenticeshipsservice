using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Testing;
using System;
using System.Threading.Tasks;
using Fix = SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.Commitments.CohortApprovalRequestedByProviderHandlerTestsFixture;
using FluentAssertions;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.EventHandlers.Cohort;
using SFA.DAS.HashingService;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.Commitments
{
    [TestFixture, Parallelizable]
    public class CohortApprovalRequestedHandlerTests : FluentTest<CohortApprovalRequestedByProviderHandlerTestsFixture>
    {        
        [Test]
        public Task Handle_WhenAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewCohort()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyAccountDocumentSavedWithCohort());
        }

        [Test]
        public Task Handle_WhenAccountDoesNotContainOrganisation_ThenAccountDocumentIsSavedWithNewCohort()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(f.Event.AccountId), f => f.Handle(),
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

    public class CohortApprovalRequestedByProviderHandlerTestsFixture : EventHandlerTestsFixture<CohortApprovalRequestedByProvider, CohortApprovalRequestedByProviderHandler>
    {
        public Mock<IAccountDocumentService> MockAccountDocumentService { get; set; }
        public Mock<IProviderCommitmentsApi> MockProviderCommitmentsApi { get; set; }
        public Mock<IHashingService> MockHashingService { get; set; }
        public CommitmentView Commitment { get; set; }
        public CommitmentView ExpectedCommitment { get; set; }
        public AccountDocument AccountDocument { get; set; }
        public AccountDocument OriginalAccountDocument { get; set; }

        public const long AccountId = 456L;
        public const string ProviderCommitmentsApiExceptionMessage = "Test message";        

        public CohortApprovalRequestedByProviderHandlerTestsFixture()
        {
            Commitment = Fixture.Create<CommitmentView>();

            MockProviderCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            MockProviderCommitmentsApi.Setup(m => m.GetProviderCommitment(Commitment.ProviderId.Value, Commitment.Id)).ReturnsAsync(Commitment);

            MockAccountDocumentService = new Mock<IAccountDocumentService>();
            MockAccountDocumentService.Setup(m => m.GetOrCreate(AccountId, CancellationToken)).ReturnsAsync(new AccountDocument(AccountId));

            MockHashingService = new Mock<IHashingService>();
            MockHashingService
                .Setup(m => m.DecodeValue(Commitment.AccountLegalEntityPublicHashedId))
                .Returns(AccountId);

            Handler = new CohortApprovalRequestedByProviderHandler(MockAccountDocumentService.Object, MockProviderCommitmentsApi.Object, MockHashingService.Object);

            Event = new CohortApprovalRequestedByProvider { AccountId = AccountId, ProviderId = Commitment.ProviderId.Value, CommitmentId = Commitment.Id };
        }

        public override Task Handle()
        {
            ExpectedCommitment = Commitment.Clone();
            OriginalAccountDocument = AccountDocument.Clone();

            return base.Handle();
        }

        public CohortApprovalRequestedByProviderHandlerTestsFixture ArrangeEmptyAccountDocument(long accountId)
        {
            AccountDocument = JsonConvert.DeserializeObject<AccountDocument>($"{{\"Account\": {{\"Id\": {accountId} }}}}");

            MockAccountDocumentService.Setup(s => s.GetOrCreate(accountId, CancellationToken)).ReturnsAsync(AccountDocument);

            return this;
        }

        public CohortApprovalRequestedByProviderHandlerTestsFixture ArrangeAccountDocumentContainsOrganisation()
        {
            var organisation = SetUpAccountDocumentWithOrganisation(Event.AccountId, AccountId);
            organisation.Cohorts = new List<Cohort>();

            MockAccountDocumentService.Setup(s => s.GetOrCreate(Event.AccountId, CancellationToken)).ReturnsAsync(AccountDocument);

            return this;
        }

        public CohortApprovalRequestedByProviderHandlerTestsFixture ArrangeProviderCommitmentApiThrowsException()
        {
            MockProviderCommitmentsApi.Setup(c => c.GetProviderCommitment(Event.ProviderId, Event.CommitmentId))
                .ThrowsAsync(new Exception(ProviderCommitmentsApiExceptionMessage));

            return this;
        }

        public CohortApprovalRequestedByProviderHandlerTestsFixture ArrangeAccountDocumentContainsCohort()
        {
            var organisation = SetUpAccountDocumentWithOrganisation(Event.AccountId, AccountId);

            var cohort = organisation.Cohorts.RandomElement();
            cohort.Id = Event.CommitmentId.ToString();
            cohort.Reference = Commitment.Reference;
            cohort.Apprenticeships = cohort.Apprenticeships.Zip(Commitment.Apprenticeships,
                (accountDocumentApprenticeship, apiApprenticeship) =>
                {
                    accountDocumentApprenticeship.Id = apiApprenticeship.Id;
                    return accountDocumentApprenticeship;
                }).ToList();

            MockAccountDocumentService.Setup(s => s.GetOrCreate(Event.AccountId, CancellationToken)).ReturnsAsync(AccountDocument);

            return this;
        }

        public Organisation SetUpAccountDocumentWithOrganisation(long accountId, long accountLegalEntityId)
        {
            AccountDocument = Fixture.Create<AccountDocument>();

            AccountDocument.Account.Id = accountId;

            AccountDocument.Deleted = null;
            AccountDocument.Account.Deleted = null;

            var organisation = AccountDocument.Account.Organisations.RandomElement();
            organisation.AccountLegalEntityId = accountLegalEntityId;

            return organisation;
        }

        public void VerifyAccountDocumentSavedWithCohort()
        {
            MockAccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => AccountIsAsExpected(d)), CancellationToken), Times.Once);
        }

        public bool AccountIsAsExpected(AccountDocument document)
        {
            var expectedAccount = GetExpectedAccount(OriginalEvent.AccountId);
            var expectedOrganisation = GetExpectedOrganisation(
                expectedAccount, AccountId, ExpectedCommitment.LegalEntityName);
            var expectedCohort = GetExpectedCohort(expectedOrganisation);

            expectedCohort.Reference = ExpectedCommitment.Reference;

            bool accountIsAsExpected = AccountIsAsExpected(expectedAccount, document);

            // this is a workaround for a bug in CompareNetObjects, where it believes equal apprenticeships are not equal
            // so we catch that case with this fluent assertion (and we exclude Apprenticeships from the CompareNetObjects comparison)
            document.Account.Organisations.Should().BeEquivalentTo(expectedAccount.Organisations);

            return accountIsAsExpected;
        }

        public bool AccountIsAsExpected(Account expectedAccount, AccountDocument savedAccountDocument)
        {
            if (savedAccountDocument?.Account == null)
                return false;

            var (accountIsAsExpected, differences) = savedAccountDocument.Account.IsEqual(expectedAccount);
            if (!accountIsAsExpected)
            {
                TestContext.WriteLine($"Saved account is not as expected: {differences}");
            }

            return accountIsAsExpected;
        }

        public Account GetExpectedAccount(long accountId)
        {
            if (OriginalAccountDocument == null)
            {
                return new Account
                {
                    Id = accountId,
                };
            }
            return OriginalAccountDocument.Account;
        }

        public Organisation GetExpectedOrganisation(Account expectedAccount, long accountLegalEntityId, string accountLegalEntityName)
        {
            if (OriginalAccountDocument != null && OriginalAccountDocument.Account.Organisations.Any())
                return expectedAccount.Organisations.Single(o => o.AccountLegalEntityId == accountLegalEntityId);

            var expectedOrganisation = new Organisation
            {
                AccountLegalEntityId = accountLegalEntityId,
                Name = accountLegalEntityName
            };
            expectedAccount.Organisations.Add(expectedOrganisation);

            return expectedOrganisation;
        }

        public Cohort GetExpectedCohort(Organisation expectedOrganisation)
        {
            Cohort expectedCohort;
            if (OriginalAccountDocument == null
                || !OriginalAccountDocument.Account.Organisations.Any())
            {
                expectedCohort = new Cohort
                {
                    Id = OriginalEvent.CommitmentId.ToString(),
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

            expectedCohort = expectedOrganisation.Cohorts.SingleOrDefault(r => r.Id == OriginalEvent.CommitmentId.ToString());
            if (expectedCohort == null)
            {
                expectedCohort = new Cohort
                {
                    Id = OriginalEvent.CommitmentId.ToString(),
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
