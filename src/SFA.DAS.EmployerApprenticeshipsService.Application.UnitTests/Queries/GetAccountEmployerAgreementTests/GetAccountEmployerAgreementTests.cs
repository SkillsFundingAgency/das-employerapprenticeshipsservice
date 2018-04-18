using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.HashingService;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using EmployerAgreementStatus = SFA.DAS.EAS.Domain.Models.EmployerAgreement.EmployerAgreementStatus;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountEmployerAgreementTests
{
    [TestFixture]
    public class GetAccountEmployerAgreementTests : FluentTest<GetAccountEmployerAgreementTestFixtures>
    {
        [Test]
        public Task Handle_WithAnyRequest_ShouldCallValidator()
        {
            return base.RunAsync(
                arrange: fixtures => fixtures
                                    .WithRequestedAccount(123, "ABC123"),
                act: fixtures => fixtures.Handle(),
                assert: fixtures => fixtures.ValidatorMock.Verify(v => v.ValidateAsync(fixtures.Request), Times.Once));
        }

        [Test]
        public void Handle_WithUnauthorisedRequest_ShouldThrowUnauthorizedAccessException()
        {
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => base.RunAsync(
                arrange: fixtures => fixtures
                                    .WithUnauthorisedRequest()
                                    .WithRequestedAccount(123, "ABC123"),
                act: fixtures => fixtures.Handle(),
                assert: fixtures => Assert.IsNotNull(fixtures.Response)));
        }

        [Test]
        public Task Handle_WithAccountThatHasOneSignedAgreement_ShouldReturnSignedAgreementAndNoPendingAgreement()
        {
            return base.RunAsync(
                arrange: fixtures => fixtures
                                        .WithRequestedAccount(123, "ABC123")
                                        .WithLegalEntityId(456)
                                        .WithSignedAgreement(123, 456, 1),
                act: fixtures => fixtures.Handle(),
                assert: fixtures => TestAgreementVersions(fixtures, 456, 1, null));
        }

        [Test]
        public Task Handle_WithAccountThatHasTwoSignedAgreements_ShouldReturnHighestVersionSignedAgreementAndNoPendingAgreement()
        {
            return base.RunAsync(
                arrange: fixtures => fixtures
                                        .WithRequestedAccount(123, "ABC123")
                                        .WithLegalEntityId(456)
                                        .WithSignedAgreement(123, 456, 1)
                                        .WithSignedAgreement(123, 456, 2),
                act : fixtures => fixtures.Handle(), 
                assert: fixtures => TestAgreementVersions(fixtures, 456, 2, null));
        }

        [Test]
        public Task Handle_WithAccountThatHasOnePendingAgreement_ShouldReturnPendingAgreementAndNoSignedAgreement()
        {
            return base.RunAsync(
                arrange: fixtures => fixtures
                    .WithRequestedAccount(123, "ABC123")
                    .WithLegalEntityId(456)
                    .WithPendingAgreement(123, 456, 1),
                act: fixtures => fixtures.Handle(),
                assert: fixtures => TestAgreementVersions(fixtures, 456, null, 1));
        }

        [Test]
        public Task Handle_WithAccountThatHasTwoPendingAgreements_ShouldReturnHighestVersionPendingAgreementAndNoSignedAgreement()
        {
            return base.RunAsync(
                arrange: fixtures => fixtures
                    .WithRequestedAccount(123, "ABC123")
                    .WithLegalEntityId(456)
                    .WithPendingAgreement(123, 456, 1)
                    .WithPendingAgreement(123, 456, 2),
                act: fixtures => fixtures.Handle(),
                assert: fixtures => TestAgreementVersions(fixtures, 456, null, 2));
        }

        [Test]
        public Task Handle_WithAccountThatHasOneSignedAndAnEarlierPendingAgreement_ShouldReturnSignedAndNoPendingAgreement()
        {
            return base.RunAsync(
                arrange: fixtures => fixtures
                    .WithRequestedAccount(123, "ABC123")
                    .WithLegalEntityId(456)
                    .WithPendingAgreement(123, 456, 1)
                    .WithSignedAgreement(123, 456, 2),
                act: fixtures => fixtures.Handle(),
                assert: fixtures => TestAgreementVersions(fixtures, 456, 2, null));
        }


        [Test]
        public Task Handle_WithAccountThatHasOneSignedAndALaterPendingAgreement_ShouldReturnSignedAndPendingAgreement()
        {
            return base.RunAsync(
                arrange: fixtures => fixtures
                    .WithRequestedAccount(123, "ABC123")
                    .WithLegalEntityId(456)
                    .WithPendingAgreement(123, 456, 3)
                    .WithSignedAgreement(123, 456, 2),
                act: fixtures => fixtures.Handle(),
                assert: fixtures => TestAgreementVersions(fixtures, 456, 2, 3));
        }

        private void TestAgreementVersions(
            GetAccountEmployerAgreementTestFixtures fixtures, 
            int legalEntityId,
            int? expectedSignedVersion,
            int? expectedPendingVersion)
        {

            var response = fixtures.Response;
            Assert.IsNotNull(response,"The query handler did not return an object - returned null");
            var agreementStatus = response.EmployerAgreements.SingleOrDefault(ea => ea.LegalEntityId == legalEntityId);
            Assert.IsNotNull(agreementStatus, "Did not receive an agreement for the expected legal entity");
            Assert.AreEqual(expectedSignedVersion, agreementStatus.SignedVersion, "The signed version number is not correct");
            Assert.AreEqual(expectedPendingVersion, agreementStatus.PendingVersion, "The pending version number is not correct");

            Assert.AreEqual(expectedSignedVersion.HasValue, agreementStatus.HasSignedAgreement, "The agreement summary has a signed agreement but one was not expected");
            Assert.AreEqual(expectedPendingVersion.HasValue, agreementStatus.HasPendingAgreement, "The agreement summary has a pending agreement but one was not expected");
        }
    }

    public class GetAccountEmployerAgreementTestFixtures
    {
        public GetAccountEmployerAgreementTestFixtures()
        {
            HashingServiceMock = new Mock<IHashingService>();
            EmployerAccountDbContextMock = new Mock<EmployerAccountDbContext>();

            Accounts = new List<Domain.Data.Entities.Account.Account>();
            AgreementTemplates = new List<AgreementTemplate>();
            EmployerAgreements = new List<EmployerAgreement>();
            LegalEntities = new List<LegalEntity>();

            ValidatorMock = new Mock<IValidator<GetAccountEmployerAgreementsRequest>>();
            ValidationResult = new ValidationResult();
        }

        public Mock<IHashingService> HashingServiceMock { get; }
        public IHashingService HashingService => HashingServiceMock.Object;

        public Mock<EmployerAccountDbContext> EmployerAccountDbContextMock { get; set; }
        public EmployerAccountDbContext EmployerAccountDbContext => EmployerAccountDbContextMock.Object;

        public List<Domain.Data.Entities.Account.Account> Accounts { get; }
        public List<AgreementTemplate> AgreementTemplates { get; set; }
        public List<EmployerAgreement> EmployerAgreements { get; }
        public List<LegalEntity> LegalEntities { get; }

        public Mock<IValidator<GetAccountEmployerAgreementsRequest>> ValidatorMock { get; }
        public IValidator<GetAccountEmployerAgreementsRequest> Validator => ValidatorMock.Object;

        public string RequestHashedAccountId { get; set; }

        public ValidationResult ValidationResult { get; }

        public GetAccountEmployerAgreementTestFixtures WithUnauthorisedRequest()
        {
            ValidationResult.IsUnauthorized = true;

            return this;
        }

        public GetAccountEmployerAgreementTestFixtures WithRequestedAccount(long accountId, string hashedAccountId)
        {
            RequestHashedAccountId = hashedAccountId;
            return WithAccount(accountId, hashedAccountId);
        }

        public GetAccountEmployerAgreementTestFixtures WithAccount(long accountId, string hashedId)
        {
            Accounts.Add(new Domain.Data.Entities.Account.Account
            {
                Id =  accountId,
            });

            HashingServiceMock.Setup(c => c.DecodeValue(hashedId)).Returns(accountId);
            HashingServiceMock.Setup(c => c.HashValue(accountId)).Returns(hashedId);

            return this;
        }

        public GetAccountEmployerAgreementTestFixtures WithLegalEntityId(long legalEntityId)
        {
            LegalEntities.Add(new LegalEntity
            {
                Id = legalEntityId,
                Name = $"{legalEntityId}"
            });

            return this;
        }

        public GetAccountEmployerAgreementTestFixtures WithSignedAgreement(long accountId, long legalEntityId, int agreementVersion)
        {
            return WithAgreement(accountId, legalEntityId, agreementVersion, EmployerAgreementStatus.Signed);
        }

        public GetAccountEmployerAgreementTestFixtures WithPendingAgreement(long accountId, long legalEntityId, int agreementVersion)
        {
            return WithAgreement(accountId, legalEntityId, agreementVersion, EmployerAgreementStatus.Pending);
        }

        public GetAccountEmployerAgreementTestFixtures WithAgreement(long accountId, long legalEntityId, int agreementVersion, SFA.DAS.EAS.Domain.Models.EmployerAgreement.EmployerAgreementStatus status)
        {
            var account = Accounts.Single(ac => ac.Id == accountId);
            var legalEntity = LegalEntities.Single(le => le.Id == legalEntityId);
            var template = AgreementTemplates.FirstOrDefault(ag => ag.VersionNumber == agreementVersion);
            if (template == null)
            {
                AgreementTemplates.Add(template = new AgreementTemplate {VersionNumber = agreementVersion});
            }

            var employerAgreement = new EmployerAgreement
            {
                Id = accountId,
                Account = account,
                AccountId = account.Id,
                LegalEntity = legalEntity,
                LegalEntityId = legalEntity.Id,
                Template = template,
                TemplateId = template.Id,
                StatusId = status
            };

            EmployerAgreements.Add(employerAgreement);

            template.Agreements.Add(employerAgreement);

            return this;
        }

        public GetAccountEmployerAgreementsRequest CreateRequest()
        {
            var query = new GetAccountEmployerAgreementsRequest
            {
                ExternalUserId = "123",
                HashedAccountId = RequestHashedAccountId
            };

            return query;
        }

        public GetAccountEmployerAgreementsQueryHandler CreateQueryHandler()
        {
            DbSetStub<Domain.Data.Entities.Account.Account> accountsDbSet = new DbSetStub<Domain.Data.Entities.Account.Account>(Accounts);
            DbSetStub<EmployerAgreement> agreementsDbSet = new DbSetStub<EmployerAgreement>(EmployerAgreements);
            DbSetStub<LegalEntity> legalEntityDbSet = new DbSetStub<LegalEntity>(LegalEntities);

            EmployerAccountDbContextMock
                .Setup(db => db.Accounts)
                .Returns(accountsDbSet);

            EmployerAccountDbContextMock
                .Setup(db => db.Agreements)
                .Returns(agreementsDbSet);

            ValidatorMock
                .Setup(x => x.ValidateAsync(Request))
                .ReturnsAsync(ValidationResult);

            var queryHandler = new GetAccountEmployerAgreementsQueryHandler(EmployerAccountDbContext, HashingService, Validator);

            return queryHandler;
        }

        public GetAccountEmployerAgreementsRequest Request { get; private set; }

        public GetAccountEmployerAgreementsResponse Response { get; private set; }

        public async Task Handle()
        {
            Request = CreateRequest();
            var queryHandler = CreateQueryHandler();
            Response = await queryHandler.Handle(Request);
        }
    }
}
