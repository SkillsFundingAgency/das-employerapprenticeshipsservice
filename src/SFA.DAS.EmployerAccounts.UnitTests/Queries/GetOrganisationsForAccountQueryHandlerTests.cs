using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationsForAccount;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;
using OrganisationType= SFA.DAS.Common.Domain.Types.OrganisationType;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries
{
    [TestFixture]
    public class GetOrganisationsForAccountQueryHandlerTests
    {
        [Test]
        public void Constructor_Valid_ShouldNotThrowException()
        {
            var fixtures = new GetOrganisationsForAccountQueryHandlerTestFixtures();

            fixtures.CreateHandler();

            Assert.Pass("Did not get exception");
        }

        [Test]
        public void Handle_NoSignedAgreementWithOneLegalEntity_ShouldNotAllowLastLegalEntityToBeRemoved()
        {
            var fixtures = new GetOrganisationsForAccountQueryHandlerTestFixtures()
                .WithAccount(123, "ABC123")
                .WithPendingLegalEntity(200, "ACME Fireworks Ltd");

            fixtures.ExecuteHandler();

            fixtures.AssertCanNotBeRemoved(200);
        }

        [Test]
        public void Handle_NoSignedAgreementWithTwoLegalEntities_ShouldAllowBothToBeRemoved()
        {
            var fixtures = new GetOrganisationsForAccountQueryHandlerTestFixtures()
                .WithAccount(123, "ABC123")
                .WithPendingLegalEntity(200, "ACME Fireworks Ltd")
                .WithPendingLegalEntity(201, "ACME Cement Ltd");

            fixtures.ExecuteHandler();

            fixtures.AssertCanBeRemoved(200, 201);
        }

        [Test]
        public void Handle_SignedAgreementWithOneLegalEntityAndNoCommitments_ShouldNotAllowLastLegalEntityToBeRemoved()
        {
            var fixtures = new GetOrganisationsForAccountQueryHandlerTestFixtures()
                .WithAccount(123, "ABC123")
                .WithSignedLegalEntity(200, "ACME Fireworks Ltd");

            fixtures.ExecuteHandler();

            fixtures.AssertCanNotBeRemoved(200);
        }

        [Test]
        public void Handle_SignedAgreementWithTwoLegalEntitiesWithCommitments_ShouldNotAllowEitherToBeRemoved()
        {
            var fixtures = new GetOrganisationsForAccountQueryHandlerTestFixtures()
                .WithAccount(123, "ABC123")
                .WithSignedLegalEntity(200, "ACME Fireworks Ltd")
                .WithSignedLegalEntity(201, "ACME Cement Ltd")
                .WithCommitment(200, "ABC/123456", OrganisationType.CompaniesHouse, 1, 1, 1)
                .WithCommitment(201, "XYZ/123456", OrganisationType.CompaniesHouse, 1, 1, 1);

            fixtures.ExecuteHandler();

            fixtures.AssertCanNotBeRemoved(200, 201);
        }

        [Test]
        public void Handle_SignedAgreementWithTwoLegalEntitiesWithZeroCommitments_ShouldAllowBothToBeRemoved()
        {
            var fixtures = new GetOrganisationsForAccountQueryHandlerTestFixtures()
                .WithAccount(123, "ABC123")
                .WithSignedLegalEntity(200, "ACME Fireworks Ltd")
                .WithSignedLegalEntity(201, "ACME Cement Ltd")
                .WithCommitment(200, "ABC/123456", OrganisationType.CompaniesHouse, 0, 0, 0)
                .WithCommitment(201, "XYZ/123456", OrganisationType.CompaniesHouse, 0, 0, 0);

            fixtures.ExecuteHandler();

            fixtures.AssertCanBeRemoved(200, 201);
        }

        [Test]
        public void Handle_SignedAgreementWithTwoLegalEntitiesWithOneCommitment_ShouldAllowNoCommitmentsToBeRemoved()
        {
            var fixtures = new GetOrganisationsForAccountQueryHandlerTestFixtures()
                .WithAccount(123, "ABC123")
                .WithSignedLegalEntity(200, "ACME Fireworks Ltd")
                .WithSignedLegalEntity(201, "ACME Cement Ltd")
                .WithCommitment(200, "ABC/123456", OrganisationType.CompaniesHouse, 1, 1, 1)
                .WithCommitment(201, "XYZ/123456", OrganisationType.CompaniesHouse, 0, 0, 0);

            fixtures.ExecuteHandler();

            fixtures.AssertCanNotBeRemoved(200);
            fixtures.AssertCanBeRemoved(201);
        }

        [Test]
        public void Handle_AnyActiveCommitments_ShouldBlockRemoval()
        {
            var fixtures = new GetOrganisationsForAccountQueryHandlerTestFixtures()
                .WithAccount(123, "ABC123")
                .WithSignedLegalEntity(200, "ACME Fireworks Ltd")
                .WithSignedLegalEntity(201, "ACME Cement Ltd")
                .WithSignedLegalEntity(202, "ACME Bird Food Ltd")
                .WithCommitment(200, "ABC/123456", OrganisationType.CompaniesHouse, 1, 0, 0)
                .WithCommitment(201, "XYZ/123456", OrganisationType.CompaniesHouse, 0, 2, 0)
                .WithCommitment(202, "RST/123456", OrganisationType.CompaniesHouse, 0, 0, 3);

            fixtures.ExecuteHandler();

            fixtures.AssertCanNotBeRemoved(200, 201, 202);
        }

        [Test]
        public void Handle_CommitmentsForDifferentOrganisationTypes_ShouldNotBeApplied()
        {
            var fixtures = new GetOrganisationsForAccountQueryHandlerTestFixtures()
                .WithAccount(123, "ABC123")
                .WithSignedLegalEntity(200, "ACME Fireworks Ltd")
                .WithSignedLegalEntity(201, "ACME Cement Ltd")
                .WithSignedLegalEntity(202, "ACME Bird Food Ltd")
                .WithCommitment(200, "ABC/123456", OrganisationType.CompaniesHouse, 1, 0, 0)
                .WithCommitment(201, "XYZ/123456", OrganisationType.CompaniesHouse, 0, 2, 0)
                .WithCommitment(202, "RST/123456", OrganisationType.CompaniesHouse, 0, 0, 3)
                // This will re-set the org type on the legal entity record
                .SetLegalEntityCode(202, "RST/123456", OrganisationType.Charities);

            fixtures.ExecuteHandler();

            fixtures.AssertCanNotBeRemoved(200, 201);
            fixtures.AssertCanBeRemoved(202);
        }
    }

    internal class GetOrganisationsForAccountQueryHandlerTestFixtures
    {
        public GetOrganisationsForAccountQueryHandlerTestFixtures()
        {
            EmployerAgreementRepositoryMock = new Mock<IEmployerAgreementRepository>();
            EmployerCommitmentApiMock = new Mock<IEmployerCommitmentApi>();
            HashingServiceMock = new Mock<IHashingService>();
            ValidatorMock = new Mock<IValidator<GetOrganisationsForAccountRequest>>();
        }

        public Mock<IEmployerAgreementRepository> EmployerAgreementRepositoryMock { get; }
        public IEmployerAgreementRepository EmployerAgreementRepository => EmployerAgreementRepositoryMock.Object;

        public Mock<IEmployerCommitmentApi> EmployerCommitmentApiMock { get; }
        public IEmployerCommitmentApi EmployerCommitmentApi => EmployerCommitmentApiMock.Object;

        public Mock<IHashingService> HashingServiceMock { get; }
        public IHashingService HashingService => HashingServiceMock.Object;

        public Mock<IValidator<GetOrganisationsForAccountRequest>> ValidatorMock { get; }
        public IValidator<GetOrganisationsForAccountRequest> Validator => ValidatorMock.Object;

        public GetOrganisationsForAccountQueryHandler CreateHandler()
        {
            return new GetOrganisationsForAccountQueryHandler(Validator, EmployerAgreementRepository, HashingService,
                EmployerCommitmentApi);
        }

        public GetOrganisationsForAccountQueryHandlerTestFixtures ExecuteHandler()
        {
            ValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<GetOrganisationsForAccountRequest>()))
                .Returns(() => Task.FromResult(new ValidationResult()));

            EmployerAgreementRepositoryMock
                .Setup(repository => repository.GetLegalEntitiesLinkedToAccount(AccountId, false))
                .ReturnsAsync(LegalEntities);

            EmployerCommitmentApiMock
                .Setup(ec => ec.GetEmployerAccountSummary(AccountId))
                .ReturnsAsync(Commitments);

            var handler = CreateHandler();

            var task = handler.Handle(new GetOrganisationsForAccountRequest {HashedAccountId = HashedAccountId});

            task.Wait(Debugger.IsAttached ? -1 : 5 * 1000);

            QueryResponse = task.Result;

            return this;
        }

        public GetOrganisationsForAccountQueryHandlerTestFixtures AssertHasQueryReponse()
        {
            Assert.IsNotNull(QueryResponse, "Query Response is null and cannot be tested");
            return this;
        }

        public GetOrganisationsForAccountQueryHandlerTestFixtures AssertCanBeRemoved(params long[] accountLegalEntityIds)
        {
            return AssertRemovalFlag(true, accountLegalEntityIds);
        }

        public GetOrganisationsForAccountQueryHandlerTestFixtures AssertCanNotBeRemoved(params long[] accountLegalEntityIds)
        {
            return AssertRemovalFlag(false, accountLegalEntityIds);
        }

        private GetOrganisationsForAccountQueryHandlerTestFixtures AssertRemovalFlag(bool expectedCanBeRemoved, params long[] accountLegalEntityIds)
        {
            AssertHasQueryReponse();

            foreach (var accountLegalEntityId in accountLegalEntityIds)
            {
                var organisation =
                    QueryResponse.Organisation.FirstOrDefault(o => o.AccountLegalEntityId == accountLegalEntityId);

                Assert.IsNotNull(organisation, $"Did not find organisation {accountLegalEntityId} in the query response - cannot make assertion");

                Assert.AreEqual(expectedCanBeRemoved, organisation.CanBeRemoved, $"organisation {organisation.AccountLegalEntityId} is not correct");
            }

            return this;
        }

        public GetOrganisationsForAccountResponse QueryResponse { get; private set; }
        public long AccountId { get; private set; }
        public string HashedAccountId { get; set; }
        public List<AccountSpecificLegalEntity> LegalEntities { get; } = new List<AccountSpecificLegalEntity>();
        public List<ApprenticeshipStatusSummary> Commitments { get; } = new List<ApprenticeshipStatusSummary>();

        public GetOrganisationsForAccountQueryHandlerTestFixtures WithAccount(long accountId, string hashedAccountId)
        {
            HashingServiceMock
                .Setup(hashingService => hashingService.DecodeValue(hashedAccountId))
                .Returns(accountId);

            AccountId = accountId;
            HashedAccountId = hashedAccountId;

            return this;
        }

        public GetOrganisationsForAccountQueryHandlerTestFixtures WithPendingLegalEntity(long accountLegalEntityId, string name)
        {
            return WithLegalEntity(accountLegalEntityId, name, 2000 + accountLegalEntityId, null);
        }

        public GetOrganisationsForAccountQueryHandlerTestFixtures WithSignedLegalEntity(long accountLegalEntityId, string name)
        {
            return WithLegalEntity(accountLegalEntityId, name, null, 3000 + accountLegalEntityId);
        }

        public GetOrganisationsForAccountQueryHandlerTestFixtures WithCommitment(
            long accountLegalEntityId, 
            string legalEntityIdentifier, 
            OrganisationType legalEntityOrganisationType, 
            int activeCount, 
            int pendingApprovalCount, 
            int pausedCount)
        {

            SetLegalEntityCode(accountLegalEntityId, legalEntityIdentifier, legalEntityOrganisationType);

            Commitments.Add(new ApprenticeshipStatusSummary
            {
                ActiveCount = activeCount,
                PendingApprovalCount = pendingApprovalCount,
                PausedCount = pausedCount,
                LegalEntityIdentifier = legalEntityIdentifier,
                LegalEntityOrganisationType = legalEntityOrganisationType
            });

            return this;
        }

        public GetOrganisationsForAccountQueryHandlerTestFixtures SetLegalEntityCode(long accountLegalEntityId, string legalEntityIdentifier, OrganisationType legalEntityOrganisationType)
        {
            var accountLegalEntity = LegalEntities.FirstOrDefault(ale => ale.AccountLegalEntityId == accountLegalEntityId);

            Assert.IsNotNull(accountLegalEntity,
                $"Commitment can not be set up for account legal entity {accountLegalEntity} because that account legal entity has not been set up yet.");

            accountLegalEntity.Source = legalEntityOrganisationType;
            accountLegalEntity.Code = legalEntityIdentifier;

            return this;
        }

        public GetOrganisationsForAccountQueryHandlerTestFixtures WithLegalEntity(long accountLegalEntityId, string name, long? pendingAgreementId, long? signedAgreementId)
        {
            LegalEntities.Add(new AccountSpecificLegalEntity
            {
                Id = 1000 + accountLegalEntityId,
                AccountLegalEntityId = accountLegalEntityId,
                Name = name,
                SignedAgreementId = signedAgreementId,
                PendingAgreementId = pendingAgreementId
            });

            return this;
        }

    }
}
