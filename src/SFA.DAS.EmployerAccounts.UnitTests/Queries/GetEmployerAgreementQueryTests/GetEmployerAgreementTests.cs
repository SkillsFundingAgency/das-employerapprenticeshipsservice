﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.TestCommon;
using SFA.DAS.Validation;
using FluentTestFixture = SFA.DAS.Testing.FluentTestFixture;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAgreementQueryTests
{
    [TestFixture]
    class GetEmployerAgreementTests : Testing.FluentTest<GetEmployerAgreementTestFixtures>
    {
        const long AccountId = 1;
        const long LegalEntityId = 2;
        const long AccountLegalEntityId = 3;
        const string HashedAccountId = "ACC123";

        [Test]
        public Task GetAgreementToSign_IfUserIsNotAuthorized_DoNotShowAgreement()
        {
            User user = null;
            EmployerAgreement signedAgreement = null;

            return RunAsync(
                arrange: fixtures => fixtures
                    .WithAccount(AccountId, HashedAccountId)
                    .WithSignedAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 3, DateTime.Now.AddDays(-20), out signedAgreement)
                    .WithUser(AccountId, "Buck", "Rogers", out user)
                    .WithCallerAsUnauthorizedUser(),
                act: fixtures => fixtures.Handle(HashedAccountId, signedAgreement.Id, user.Ref),
                assert: (fixturesf, r) => r.ShouldThrowExactly<UnauthorizedAccessException>());
        }

        [Test]
        public Task GetAgreementToSign_IfRequestIsNotValid_DoNotShowAgreement()
        {
            User user = null;
            EmployerAgreement signedAgreement = null;

            return RunAsync(
                arrange: fixtures => fixtures
                    .WithAccount(AccountId, HashedAccountId)
                    .WithSignedAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 3, DateTime.Now.AddDays(-20), out signedAgreement)
                    .WithUser(AccountId, "Buck", "Rogers", out user)
                    .WithInvalidRequest(),
                act: fixtures => fixtures.Handle(HashedAccountId, signedAgreement.Id, user.Ref),
                assert: (f, r) => r.ShouldThrowExactly<InvalidRequestException>());
        }

        [Test]
        public Task GetAgreementToSign_IfNoAgreementFound_ShouldReturn()
        {
            return RunAsync(
                act: fixtures => fixtures.Handle("ACC123", 1, Guid.NewGuid()),
                assert: fixtures =>
                {
                    Assert.IsNull(fixtures.Response.EmployerAgreement);
                    Assert.IsNull(fixtures.Response.LastSignedAgreement);
                }
            );
        }

        [Test]
        public Task GetAgreementToSign_ShouldReturnRequestedAgreement()
        {
            EmployerAgreement expectedAgreement = null;
            User user = null;

            return RunAsync(
                arrange: fixtures => fixtures
                                        .WithAccount(AccountId, HashedAccountId)
                                        .WithAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 1, EmployerAgreementStatus.Pending, out expectedAgreement)
                                        .WithAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 2, EmployerAgreementStatus.Pending, out _)
                                        .WithUser(AccountId, "Buck", "Rogers", out user),
                act: fixtures => fixtures.Handle(HashedAccountId, expectedAgreement.Id, user.Ref),
                assert: fixtures => Assert.AreEqual(expectedAgreement.Id, fixtures.Response.EmployerAgreement.Id));
        }

        [Test]
        public Task GetAgreementToSign_ShouldReturnLatestSignedAgreement()
        {
            EmployerAgreement latestSignedAgreement = null;
            EmployerAgreement pendingAgreement = null;
            User user = null;

            return RunAsync(
                arrange: fixtures => fixtures
                                        .WithAccount(AccountId, HashedAccountId)
                                        .WithSignedAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 1, DateTime.Now.AddDays(-60), out _)
                                        .WithSignedAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 2, DateTime.Now.AddDays(-30), out latestSignedAgreement)
                                        .WithPendingAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 3, out pendingAgreement)
                                        .WithUser(AccountId, "Buck", "Rogers", out user),
                act: fixtures => fixtures.Handle(HashedAccountId, pendingAgreement.Id, user.Ref),
                assert: fixtures =>
                {
                    Assert.AreEqual(pendingAgreement.Id, fixtures.Response.EmployerAgreement.Id);
                    Assert.AreEqual(latestSignedAgreement.Id, fixtures.Response.LastSignedAgreement.Id);
                });
        }

        [Test]
        public Task GetAgreementToSign_IfNoSignedAgreementsExists_ShouldReturnNoSignedAgreement()
        {
            User user = null;
            EmployerAgreement pendingAgreement = null;

            return RunAsync(
                arrange: fixtures => fixtures
                                        .WithAccount(AccountId, HashedAccountId)
                                        .WithPendingAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 3, out pendingAgreement)
                                        .WithUser(AccountId, "Buck", "Rogers", out user),
                act: fixtures => fixtures.Handle(HashedAccountId, pendingAgreement.Id, user.Ref),
                assert: fixtures =>
                    Assert.IsNull(fixtures.Response.LastSignedAgreement));
        }

        [Test]
        public Task GetAgreementToSign_IfSignedAgreementIfForAnotherAccount_ShouldNotReturnSignedAgreement()
        {
            User user = null;
            EmployerAgreement latestAgreement = null;

            return RunAsync(
                arrange: fixtures => fixtures
                                        .WithAccount(AccountId, HashedAccountId)
                                        .WithAccount(AccountId + 1, "XXX123")
                                        .WithPendingAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 3, out latestAgreement)
                                        .WithSignedAgreement(AccountId + 1, LegalEntityId, AccountLegalEntityId + 1, 2, DateTime.Now.AddDays(-30), out _)
                                        .WithUser(AccountId, "Buck", "Rogers", out user),
                act: fixtures => fixtures.Handle(HashedAccountId, latestAgreement.Id, user.Ref),
                assert: fixtures =>
                    Assert.IsNull(fixtures.Response.LastSignedAgreement));
        }

        [Test]
        public Task GetAgreementToSign_IfRequestAgreementIsSigned_ShouldNotReturnLatestSignedAgreementAsWell()
        {
            User user = null;
            EmployerAgreement latestAgreement = null;

            return RunAsync(
                arrange: fixtures => fixtures
                                        .WithAccount(AccountId, HashedAccountId)
                                        .WithSignedAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 3, DateTime.Now.AddDays(-10), out latestAgreement)
                                        .WithSignedAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 2, DateTime.Now.AddDays(-20), out _)
                                        .WithUser(AccountId, "Buck", "Rogers", out user),
                act: fixtures => fixtures.Handle(HashedAccountId, latestAgreement.Id, user.Ref),
                assert: fixtures =>
                    Assert.IsNull(fixtures.Response.LastSignedAgreement));
        }

        [Test]
        public Task GetAgreementToSign_IfAgreementHasNotBeenSigned_ShouldAddCurrentUserAsSigner()
        {
            User user = null;
            EmployerAgreement latestAgreement = null;

            return RunAsync(
                arrange: fixtures => fixtures
                                        .WithAccount(AccountId, HashedAccountId)
                                        .WithPendingAgreement(AccountId, LegalEntityId, AccountLegalEntityId, 2, out latestAgreement)
                                        .WithUser(AccountId, "Buck", "Rogers", out user),
                act: fixtures => fixtures.Handle(HashedAccountId, latestAgreement.Id, user.Ref),
                assert: fixtures => Assert.AreEqual(user.FullName, fixtures.Response.EmployerAgreement.SignedByName));
        }
    }

    internal class GetEmployerAgreementTestFixtures : FluentTestFixture
    {
        public Mock<IValidator<GetEmployerAgreementRequest>> Validator { get; }
        public IConfigurationProvider ConfigurationProvider { get; }

        public GetEmployerAgreementResponse Response { get; private set; }

        public GetEmployerAgreementTestFixtures()
        {
            EmployerAgreementBuilder = new EmployerAgreementBuilder();

            Validator = new Mock<IValidator<GetEmployerAgreementRequest>>();

            ConfigurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<AgreementMappings>();
                c.AddProfile<EmploymentAgreementStatusMappings>();
                c.AddProfile<LegalEntityMappings>();

            });

            Validator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAgreementRequest>()))
                     .ReturnsAsync(new ValidationResult());
        }

        public async Task Handle(string hashedAccountId, long agreementId, Guid externalUserId)
        {
            EmployerAgreementBuilder.EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities();
            EmployerAgreementBuilder.SetupMockDbContext();
            var request = BuildRequest(hashedAccountId, agreementId, externalUserId);

            var handler = new GetEmployerAgreementQueryHandler(
                new Lazy<EmployerAccountsDbContext>(() => EmployerAgreementBuilder.EmployerAccountDbContext),
                EmployerAgreementBuilder.HashingService,
                Validator.Object,
                ConfigurationProvider);

            Response = await handler.Handle(request);
        }

        public GetEmployerAgreementRequest BuildRequest(string hashedAccountId, long agreementId, Guid externalUserId)
        {
            var agreementHashId = EmployerAgreementBuilder.HashingService.HashValue(agreementId);
            var request = new GetEmployerAgreementRequest
            {
                HashedAccountId = hashedAccountId,
                AgreementId = agreementHashId,
                ExternalUserId = externalUserId.ToString()
            };

            return request;
        }

        public GetEmployerAgreementTestFixtures WithAccount(long accountId, string hashedAccountId)
        {
            EmployerAgreementBuilder.WithAccount(accountId, hashedAccountId);
            return this;
        }

        public GetEmployerAgreementTestFixtures WithUser(long accountId, string firstName, string lastName, out User user)
        {
            var account = EmployerAgreementBuilder.GetAccount(accountId);
            EmployerAgreementBuilder.WithUser(account.Id, firstName, lastName, out user);
            return this;
        }

        public GetEmployerAgreementTestFixtures WithPendingAgreement(long accountId, long legalEntityId, long accountLegalEntityId, int agreementVersion)
        {
            EmployerAgreementBuilder.WithPendingAgreement(accountId, legalEntityId, accountLegalEntityId, agreementVersion);
            return this;
        }

        public GetEmployerAgreementTestFixtures WithPendingAgreement(long accountId, long legalEntityId, long accountLegalEntityId, int agreementVersion, out EmployerAgreement employerAgreement)
        {
            EmployerAgreementBuilder.WithPendingAgreement(accountId, legalEntityId, accountLegalEntityId, agreementVersion, out employerAgreement);
            return this;
        }

        public GetEmployerAgreementTestFixtures WithSignedAgreement(long accountId, long legalEntityId, long accountLegalEntityId, int agreementVersion,DateTime signedTime, out EmployerAgreement employerAgreement)
        {
            EmployerAgreementBuilder.WithSignedAgreement(accountId, legalEntityId, accountLegalEntityId, agreementVersion, signedTime, out employerAgreement);
            return this;
        }

        public GetEmployerAgreementTestFixtures WithAgreement(long accountId, long legalEntityId, long accountLegalEntityId, int agreementVersion, EmployerAgreementStatus status, out EmployerAgreement employerAgreement)
        {
            EmployerAgreementBuilder.WithAgreement(accountId, legalEntityId, accountLegalEntityId, agreementVersion, status, out employerAgreement);

            return this;
        }

        public GetEmployerAgreementTestFixtures WithCallerAsUnauthorizedUser()
        {
            Validator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAgreementRequest>()))
                .ReturnsAsync(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>(),
                    IsUnauthorized = true
                });

            return this;
        }

        public GetEmployerAgreementTestFixtures WithInvalidRequest()
        {
            Validator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAgreementRequest>()))
                .ReturnsAsync(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        {"HashedAccountId", "Account Id is not a valid Id"}
                    },
                    IsUnauthorized = false
                });

            return this;
        }

        public void EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities()
        {
            EmployerAgreementBuilder.EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities();
        }

        private EmployerAgreementBuilder EmployerAgreementBuilder { get; }
    }
}
