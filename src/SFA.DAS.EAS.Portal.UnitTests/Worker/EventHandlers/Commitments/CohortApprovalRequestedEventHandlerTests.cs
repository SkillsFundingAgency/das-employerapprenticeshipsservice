using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
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
    }

    public class CohortApprovalRequestedByProviderEventHandlerTestsFixture : EventHandlerTestsFixture<
        CohortApprovalRequestedByProvider, CohortApprovalRequestedByProviderEventHandler>
    {
        public Mock<IProviderCommitmentsApi> ProviderCommitmentsApi { get; set; }
        public Mock<IHashingService> HashingService { get; set; }
        public CommitmentView Commitment { get; set; }
        public CommitmentView ExpectedCommitment { get; set; }
        public long CohortId = 6789L;
        public const long DecodedAccountLegalEntityId = 123L;

        public CohortApprovalRequestedByProviderEventHandlerTestsFixture() : base(false)
        {
            Commitment = Fixture.Create<CommitmentView>();
            
            ProviderCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            ProviderCommitmentsApi
                //todo: check correct providerid & commitmentid
                .Setup(m => m.GetProviderCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Commitment);

            HashingService = new Mock<IHashingService>();
            HashingService
                    //todo: real value
                .Setup(m => m.DecodeValue(It.IsAny<string>()))
                .Returns(DecodedAccountLegalEntityId);                    
            
            Handler = new CohortApprovalRequestedByProviderEventHandler(
                AccountDocumentService.Object,
                MessageContextInitialisation.Object,
                Logger.Object,
                ProviderCommitmentsApi.Object,
                HashingService.Object);

            //todo: let test use fixture generated?
            Message.CommitmentId = CohortId;
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
            Account expectedAccount;
            Cohort expectedCohort;
            
            if (AccountDocument == null)
            {
                expectedAccount = new Account
                {
                    Id = ExpectedMessage.AccountId,
                };

                var organisation = new Organisation
                {
                    AccountLegalEntityId = DecodedAccountLegalEntityId,
                    Name = ExpectedCommitment.LegalEntityName
                };
                expectedAccount.Organisations.Add(organisation);

                expectedCohort = new Cohort();
                organisation.Cohorts.Add(expectedCohort);

                expectedCohort.Apprenticeships = ExpectedCommitment.Apprenticeships.Select(ea =>
                    new Apprenticeship
                    {
                        Id = ea.Id,
                        FirstName = ea.FirstName,
                        LastName  = ea.LastName,
                        CourseName = ea.TrainingName,
                        ProposedCost = ea.Cost,
                        StartDate = ea.StartDate,
                        EndDate = ea.EndDate
                    }).ToList();
            }
            else
            {
                expectedAccount = AccountDocument.Account;
                expectedCohort = expectedAccount
                    .Organisations.Single(o => o.AccountLegalEntityId == DecodedAccountLegalEntityId)
                    .Cohorts.Single(r => r.Id == CohortId.ToString());
            }

            //todo: id should be long
            expectedCohort.Id = CohortId.ToString();
            expectedCohort.Reference = ExpectedCommitment.Reference;
            
            return document?.Account != null && document.Account.IsEqual(expectedAccount);
        }
    }
}
