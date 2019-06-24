using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Testing;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.EventHandlers.Cohort;
using SFA.DAS.HashingService;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using System.Threading;
using System.Linq;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.Commitments
{
    [TestFixture, Parallelizable]
    public class CohortApprovedByEmployerHandlerTests : FluentTest<CohortApprovedByEmployerHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenAccountDoesContainOrganisationAndCohort_ThenAccountDocumentIsSavedWithUpdatedCohort()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsCohort(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithCohortApproved());
        }
    }

    public class CohortApprovedByEmployerHandlerTestsFixture : EventHandlerTestsFixture<CohortApprovedByEmployer, CohortApprovedByEmployerHandler>
    {
        public Mock<IAccountDocumentService> MockAccountDocumentService { get; set; }
        public Mock<IProviderCommitmentsApi> MockProviderCommitmentsApi { get; set; }
        public Mock<IHashingService> MockHashingService { get; set; }
        public CommitmentView Commitment { get; set; }
        public CommitmentView ExpectedCommitment { get; set; }
        public AccountDocument AccountDocument { get; set; }
        public AccountDocument OriginalAccountDocument { get; set; }

        public const long AccountId = 456L;
        public const long AccountLegalEntityId = 456L;
        
        public CohortApprovedByEmployerHandlerTestsFixture()
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

            Handler = new CohortApprovedByEmployerHandler(MockAccountDocumentService.Object, MockProviderCommitmentsApi.Object, MockHashingService.Object);

            Event = new CohortApprovedByEmployer { AccountId = AccountId, ProviderId = Commitment.ProviderId.Value, CommitmentId = Commitment.Id };
        }

        public override Task Handle()
        {
            ExpectedCommitment = Commitment.Clone();
            OriginalAccountDocument = AccountDocument.Clone();

            return base.Handle();
        }

        public CohortApprovedByEmployerHandlerTestsFixture ArrangeAccountDocumentContainsCohort()
        {
            var organisation = SetUpAccountDocumentWithOrganisation(Event.AccountId, AccountLegalEntityId);

            organisation.Cohorts.RandomElement().Id = Event.CommitmentId.ToString();

            MockAccountDocumentService.Setup(s => s.GetOrCreate(Event.AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);

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

        public CohortApprovedByEmployerHandlerTestsFixture VerifyAccountDocumentSavedWithCohortApproved()
        {
            MockAccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => CohortIsSetToApproved(d)), It.IsAny<CancellationToken>()), Times.Once);

            return this;
        }

        private bool CohortIsSetToApproved(AccountDocument document)
        {
            return document.Account.Organisations.SelectMany(org => org.Cohorts)
                .Single(co => co.Id == Event.CommitmentId.ToString()).IsApproved;
        }
    }
}
