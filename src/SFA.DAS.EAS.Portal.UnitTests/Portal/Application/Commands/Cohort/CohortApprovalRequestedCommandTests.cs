using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Commands.Cohort;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.UnitTests.Builders;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Commands.Cohort
{
    [Parallelizable]
    [TestFixture]
    public class CohortApprovalRequestedCommandHandlerTests
    {
        public class TestContext
        {
            //todo: rename
            public CohortApprovalRequestedCommand Sut { get; private set; }
            public CohortApprovalRequestedByProvider CohortApprovalRequestedByProvider { get; private set; }
            public Fixture Fixture { get; private set; }
            public Account TestAccount { get; private set; }
            public AccountDocument TestAccountDocument { get; private set; }
            public CommitmentView TestCommitment { get; private set; }
            public Mock<IAccountDocumentService> MockAccountDocumentService { get; private set; }
            public Mock<IProviderCommitmentsApi> MockProviderCommitmentsApi { get; private set; }
            public Mock<IHashingService> MockHashingService { get; private set; }
            public Mock<ILogger<CohortApprovalRequestedCommand>> MockLogger { get; private set; }
            public long UnHashedId = 123;

            public TestContext()
            {
                TestAccount = new AccountBuilder().WithOrganisation(new OrganisationBuilder().WithId(UnHashedId));
                TestAccountDocument = new AccountDocument() { Account = TestAccount };
                TestCommitment = new CommitmentViewBuilder();

                MockAccountDocumentService = new Mock<IAccountDocumentService>();
                MockAccountDocumentService.Setup(s => s.Get(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(TestAccountDocument);
                
                MockHashingService = new Mock<IHashingService>();
                MockLogger = new Mock<ILogger<CohortApprovalRequestedCommand>>();

                MockHashingService
                    .Setup(m => m.DecodeValue(It.IsAny<string>()))
                    .Returns(UnHashedId);                    

                MockProviderCommitmentsApi = new Mock<IProviderCommitmentsApi>();

                MockProviderCommitmentsApi
                    .Setup(m => m.GetProviderCommitment(It.IsAny<long>(), It.IsAny<long>()))
                    .ReturnsAsync(TestCommitment);

                Fixture = new Fixture();
                CohortApprovalRequestedByProvider = Fixture.Create<CohortApprovalRequestedByProvider>();
                
                Sut = new CohortApprovalRequestedCommand(MockAccountDocumentService.Object, MockProviderCommitmentsApi.Object, MockHashingService.Object, MockLogger.Object);
            }
        }

        public class Handle : CohortApprovalRequestedCommandHandlerTests
        {
            [Test]
            public async Task WhenCalled_ThenTheAccountServiceIsCalledToSaveTheAccount()
            {
                // arrange
                var testContext = new TestContext();

                // act
                await testContext.Sut.Execute(testContext.CohortApprovalRequestedByProvider);

                //assert
                testContext.MockAccountDocumentService.Verify(m => m.Save(testContext.TestAccountDocument, It.IsAny<CancellationToken>()), Times.Once);
            }

            [Test]
            public async Task WhenCalledForANewCohort_ThenTheNewCohortIsSavedAgainstTheAccount()
            {
                // arrange
                var testContext = new TestContext();

                long cohortId = 456;
                testContext.TestCommitment.Id = cohortId;
                testContext.TestAccount.Organisations.First().Cohorts.Count.Should().Be(0);

                // act
                await testContext.Sut.Execute(testContext.CohortApprovalRequestedByProvider);

                //assert
                testContext.MockAccountDocumentService.Verify(m =>
                m.Save(It.Is<AccountDocument>(a =>
                a.Account.Organisations.First().Cohorts.Count.Equals(1) &&
                a.Account.Organisations.First().Cohorts.ToList().SingleOrDefault(c => c.Id.Equals(cohortId.ToString())) != null), It.IsAny<CancellationToken>()),
                Times.Once);
            }

            [Test]
            public async Task WhenCalledForAnExistingCohort_ThenTheCohortCountIsNotChanged()
            {
                // arrange
                var testContext = new TestContext();
                long cohortId = 789;
                EAS.Portal.Client.Types.Cohort cohort = new CohortBuilder().WithId(cohortId.ToString());
                testContext.TestAccount.Organisations.First().Cohorts.Add(cohort);
                testContext.TestCommitment.Id = cohortId;
                testContext.TestAccount.Organisations.First().Cohorts.Count.Should().Be(1);

                // act
                await testContext.Sut.Execute(testContext.CohortApprovalRequestedByProvider);

                //assert
                testContext.MockAccountDocumentService.Verify(m =>
                m.Save(It.Is<AccountDocument>(a =>
                a.Account.Organisations.First().Cohorts.Count.Equals(1)), It.IsAny<CancellationToken>()),
                Times.Once);
            }

            [Test]
            public async Task WhenANewCohortIsAdded_ThenApprenticeshipsInTheCohortAreStored()
            {
                // arrange
                var testContext = new TestContext();
                string cohortReference = Guid.NewGuid().ToString();
                long apprenticeshipId = 123;

                testContext.TestCommitment.Reference = cohortReference;
                testContext.TestCommitment.Apprenticeships = new List<Commitments.Api.Types.Apprenticeship.Apprenticeship>() { new Commitments.Api.Types.Apprenticeship.Apprenticeship { Id = apprenticeshipId } };

                // act
                await testContext.Sut.Execute(testContext.CohortApprovalRequestedByProvider);

                //assert
                testContext.MockAccountDocumentService.Verify(m =>
                m.Save(It.Is<AccountDocument>(a =>
                a.Account.Organisations.First().Cohorts.Count.Equals(1) &&
                a.Account.Organisations.First().Cohorts.First().Apprenticeships.Count.Equals(1) &&
                a.Account.Organisations.First().Cohorts.First().Apprenticeships.First().Id.Equals(apprenticeshipId)), It.IsAny<CancellationToken>()),
                Times.Once);
            }

            [Test]
            public async Task WhenAnExistingCohortIsHandled_ThenAnyApprenticeshipsChangesAreAlsoStored()
            {
                // arrange
                var testContext = new TestContext();
                long cohortId = 123;
                Apprenticeship apprenticeship = new ApprenticeshipBuilder();
                EAS.Portal.Client.Types.Cohort cohort = new CohortBuilder()
                    .WithId(cohortId.ToString())
                    .WithApprenticeship(apprenticeship);
                testContext.TestAccount.Organisations.First().Cohorts.Add(cohort);
                testContext.TestAccount.Organisations.First().Cohorts.Count.Should().Be(1);

                testContext.TestCommitment.Id = cohortId;
                var testApprenticeship = new Commitments.Api.Types.Apprenticeship.Apprenticeship
                {
                    Id = apprenticeship.Id,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    Cost = 123.45M,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(10),
                    TrainingName = Guid.NewGuid().ToString()
                };
                testContext.TestCommitment.Apprenticeships.Add(testApprenticeship);

                testContext.TestAccount.Organisations.First().Cohorts.First().Apprenticeships.Count.Should().Be(1);

                // act
                await testContext.Sut.Execute(testContext.CohortApprovalRequestedByProvider);

                //assert
                testContext.MockAccountDocumentService.Verify(m =>
                m.Save(It.Is<AccountDocument>(a =>
                a.Account.Organisations.First().Cohorts.First().Apprenticeships.Count.Equals(1) &&
                (a.Account.Organisations.First().Cohorts.First().Apprenticeships.First().Id == testApprenticeship.Id) &&
                a.Account.Organisations.First().Cohorts.First().Apprenticeships.First().FirstName.Equals(testApprenticeship.FirstName) &&
                a.Account.Organisations.First().Cohorts.First().Apprenticeships.First().LastName.Equals(testApprenticeship.LastName) &&
                a.Account.Organisations.First().Cohorts.First().Apprenticeships.First().CourseName.Equals(testApprenticeship.TrainingName) &&
                a.Account.Organisations.First().Cohorts.First().Apprenticeships.First().StartDate.Equals(testApprenticeship.StartDate) &&
                a.Account.Organisations.First().Cohorts.First().Apprenticeships.First().EndDate.Equals(testApprenticeship.EndDate) &&
                a.Account.Organisations.First().Cohorts.First().Apprenticeships.First().ProposedCost.Equals(testApprenticeship.Cost)
                ), It.IsAny<CancellationToken>()),
                Times.Once);
            }
        }
    }
}
