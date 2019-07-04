using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.EventHandlers.Commitments;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.Commitments
{
    [TestFixture, Parallelizable]
    public class CohortApprovedByEmployerEventHandlerTests : FluentTest<CohortApprovedByEmployerEventHandlerTestsFixture>
    {
        //todo: test coverage a bit thin
        
        [Test]
        public Task Handle_WhenAccountDoesContainOrganisationAndCohort_ThenAccountDocumentIsSavedWithUpdatedCohort()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsCohort(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithCohortApproved());
        }
    }

    public class CohortApprovedByEmployerEventHandlerTestsFixture: EventHandlerBaseTestFixture<CohortApprovedByEmployer, CohortApprovedByEmployerEventHandler>
    {
        public AccountDocHelper AccountDocHelper { get; set; }

        public CommitmentView Commitment { get; set; }
        public CommitmentView ExpectedCommitment { get; set; }
        public const long AccountLegalEntityId = 456L;

        public CohortApprovedByEmployerEventHandlerTestsFixture()
        {
            AccountDocHelper = new AccountDocHelper();

            Handler = new CohortApprovedByEmployerEventHandler(
                AccountDocHelper.AccountDocumentService.Object,
                Logger.Object);
        }

        public CohortApprovedByEmployerEventHandlerTestsFixture ArrangeAccountDocumentContainsCohort()
        {
            var organisation = AccountDocHelper.SetUpAccountDocumentWithOrganisation(
                Message.AccountId, AccountLegalEntityId);

            organisation.Cohorts.RandomElement().Id = Message.CommitmentId.ToString();
            
            AccountDocHelper.AccountDocumentService.Setup(s => s.GetOrCreate(
                Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(AccountDocHelper.AccountDocument);
            
            return this;
        }

        public override Task Handle()
        {
            ExpectedCommitment = Commitment.Clone();
            AccountDocHelper.OriginalAccountDocument = AccountDocHelper.AccountDocument.Clone();
            return base.Handle();
        }
        
        public CohortApprovedByEmployerEventHandlerTestsFixture VerifyAccountDocumentSavedWithCohortApproved()
        {
            AccountDocHelper.AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => CohortIsSetToApproved(d)),It.IsAny<CancellationToken>()),
                Times.Once);
            
            return this;
        }
        
        private bool CohortIsSetToApproved(AccountDocument document)
        {
            return document.Account.Organisations.SelectMany(org => org.Cohorts)
                .Single(co => co.Id == Message.CommitmentId.ToString()).IsApproved;
        }
    }
}