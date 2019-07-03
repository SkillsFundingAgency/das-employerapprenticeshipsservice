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

    public class CohortApprovedByEmployerEventHandlerTestsFixture
    {
        public EventHandlerTestsFixture<CohortApprovedByEmployer, CohortApprovedByEmployerEventHandler> EventHandlerTestsFixture { get; set; }

        public CommitmentView Commitment { get; set; }
        public CommitmentView ExpectedCommitment { get; set; }
        public const long AccountLegalEntityId = 456L;

        public CohortApprovedByEmployerEventHandlerTestsFixture()
        {
            EventHandlerTestsFixture = new EventHandlerTestsFixture<CohortApprovedByEmployer, CohortApprovedByEmployerEventHandler>();
        }

        public CohortApprovedByEmployerEventHandlerTestsFixture ArrangeAccountDocumentContainsCohort()
        {
            var organisation = EventHandlerTestsFixture.SetUpAccountDocumentWithOrganisation(
                EventHandlerTestsFixture.Message.AccountId, AccountLegalEntityId);

            organisation.Cohorts.RandomElement().Id = EventHandlerTestsFixture.Message.CommitmentId.ToString();
            
            EventHandlerTestsFixture.AccountDocumentService.Setup(s => s.GetOrCreate(
                EventHandlerTestsFixture.Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventHandlerTestsFixture.AccountDocument);
            
            return this;
        }

        public Task Handle()
        {
            ExpectedCommitment = Commitment.Clone();

            return EventHandlerTestsFixture.Handle();
        }
        
        public CohortApprovedByEmployerEventHandlerTestsFixture VerifyAccountDocumentSavedWithCohortApproved()
        {
            EventHandlerTestsFixture.AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => CohortIsSetToApproved(d)),It.IsAny<CancellationToken>()),
                Times.Once);
            
            return this;
        }
        
        private bool CohortIsSetToApproved(AccountDocument document)
        {
            return document.Account.Organisations.SelectMany(org => org.Cohorts)
                .Single(co => co.Id == EventHandlerTestsFixture.Message.CommitmentId.ToString()).IsApproved;
        }
    }
}