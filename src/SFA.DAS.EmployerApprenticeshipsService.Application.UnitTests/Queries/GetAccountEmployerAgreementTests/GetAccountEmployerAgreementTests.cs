using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;

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
            var agreementStatus = response.EmployerAgreements.SingleOrDefault(ea => ea.LegalEntity.Id == legalEntityId);
            Assert.IsNotNull(agreementStatus, "Did not receive an agreement for the expected legal entity");
            Assert.AreEqual(expectedSignedVersion, agreementStatus.Signed?.VersionNumber, "The signed version number is not correct");
            Assert.AreEqual(expectedPendingVersion, agreementStatus.Pending?.VersionNumber, "The pending version number is not correct");

            Assert.AreEqual(expectedSignedVersion.HasValue, agreementStatus.HasSignedAgreement, "The agreement summary has a signed agreement but one was not expected");
            Assert.AreEqual(expectedPendingVersion.HasValue, agreementStatus.HasPendingAgreement, "The agreement summary has a pending agreement but one was not expected");
        }
    }

    public class GetAccountEmployerAgreementTestFixtures : FluentTestFixture
    {
        public GetAccountEmployerAgreementTestFixtures()
        {
            ValidatorMock = new Mock<IValidator<GetAccountEmployerAgreementsRequest>>();
            ValidationResult = new ValidationResult();

            ConfigurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<EmploymentAgreementStatusMappings>();
                c.AddProfile<LegalEntityMappings>();
            });
            EmployerAgreementBuilder = new EmployerAgreementBuilder();
        }

        public Mock<IValidator<GetAccountEmployerAgreementsRequest>> ValidatorMock { get; }
        public IValidator<GetAccountEmployerAgreementsRequest> Validator => ValidatorMock.Object;

        public IConfigurationProvider ConfigurationProvider { get; }

        public string RequestHashedAccountId { get; set; }

        public ValidationResult ValidationResult { get; }

        public GetAccountEmployerAgreementTestFixtures WithLegalEntityId(long legalEntityId)
        {
            EmployerAgreementBuilder.WithLegalEntityId(legalEntityId);
            return this;
        }

        public GetAccountEmployerAgreementTestFixtures WithPendingAgreement(long accountId, long legalEntityId, int agreementVersion)
        {
            EmployerAgreementBuilder.WithPendingAgreement(accountId, legalEntityId, agreementVersion);
            return this;
        }

        public GetAccountEmployerAgreementTestFixtures WithSignedAgreement(long accountId, long legalEntityId, int agreementVersion)
        {
            EmployerAgreementBuilder.WithSignedAgreement(accountId, legalEntityId, agreementVersion, DateTime.Now.AddDays(-20), out _);
            return this;
        }

        public GetAccountEmployerAgreementTestFixtures WithSignedAgreement(long accountId, long legalEntityId, int agreementVersion, DateTime signedDateTime, out EmployerAgreement employerAgreement)
        {
            EmployerAgreementBuilder.WithSignedAgreement(accountId, legalEntityId, agreementVersion, signedDateTime, out employerAgreement);
            return this;
        }

        public GetAccountEmployerAgreementTestFixtures EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities()
        {
            EmployerAgreementBuilder.EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities();
            return this;
        }

        public GetAccountEmployerAgreementTestFixtures WithUnauthorisedRequest()
        {
            ValidationResult.IsUnauthorized = true;

            return this;
        }

        public GetAccountEmployerAgreementTestFixtures WithRequestedAccount(long accountId, string hashedAccountId)
        {
            RequestHashedAccountId = hashedAccountId;
            EmployerAgreementBuilder.WithAccount(accountId, hashedAccountId);
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
            ValidatorMock
                .Setup(x => x.ValidateAsync(Request))
                .ReturnsAsync(ValidationResult);

            var queryHandler = new GetAccountEmployerAgreementsQueryHandler(
                new Lazy<EmployerAccountsDbContext>(() => EmployerAgreementBuilder.EmployerAccountDbContext), 
                EmployerAgreementBuilder.HashingService,
                Validator, ConfigurationProvider);

            return queryHandler;
        }

        public GetAccountEmployerAgreementsRequest Request { get; private set; }

        public GetAccountEmployerAgreementsResponse Response { get; private set; }

        private EmployerAgreementBuilder EmployerAgreementBuilder { get; }

        public async Task Handle()
        {
            EmployerAgreementBuilder.EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities();
            EmployerAgreementBuilder.SetupMockDbContext();
            Request = CreateRequest();
            var queryHandler = CreateQueryHandler();
            Response = await queryHandler.Handle(Request);
        }
    }
}
