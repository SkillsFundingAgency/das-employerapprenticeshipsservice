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
        public AccountEventHandlerTestHelper<CohortApprovedByEmployer, CohortApprovedByEmployerEventHandler> Helper { get; set; }

        public CommitmentView Commitment { get; set; }
        public CommitmentView ExpectedCommitment { get; set; }
        public const long AccountLegalEntityId = 456L;

        public CohortApprovedByEmployerEventHandlerTestsFixture()
        {
            Helper = new AccountEventHandlerTestHelper<CohortApprovedByEmployer, CohortApprovedByEmployerEventHandler>();
        }

        public CohortApprovedByEmployerEventHandlerTestsFixture ArrangeAccountDocumentContainsCohort()
        {
            var organisation = Helper.SetUpAccountDocumentWithOrganisation(
                Helper.Message.AccountId, AccountLegalEntityId);

            organisation.Cohorts.RandomElement().Id = Helper.Message.CommitmentId.ToString();
            
            Helper.AccountDocumentService.Setup(s => s.GetOrCreate(
                Helper.Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Helper.AccountDocument);
            
            return this;
        }

        public Task Handle()
        {
            ExpectedCommitment = Commitment.Clone();

            return Helper.Handle();
        }
        
        public CohortApprovedByEmployerEventHandlerTestsFixture VerifyAccountDocumentSavedWithCohortApproved()
        {
            Helper.AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => CohortIsSetToApproved(d)),It.IsAny<CancellationToken>()),
                Times.Once);
            
            return this;
        }
        
        private bool CohortIsSetToApproved(AccountDocument document)
        {
            return document.Account.Organisations.SelectMany(org => org.Cohorts)
                .Single(co => co.Id == Helper.Message.CommitmentId.ToString()).IsApproved;
        }
    }
}