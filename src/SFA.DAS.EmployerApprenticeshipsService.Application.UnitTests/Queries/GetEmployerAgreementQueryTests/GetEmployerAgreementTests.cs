using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Membership = SFA.DAS.EAS.Domain.Models.AccountTeam.Membership;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementQueryTests
{
    [TestFixture]
    class GetEmployerAgreementTests : FluentTest<GetEmployerAgreementTestFixtures>
    {
        [Test]
        public Task GetAgreementToSign_IfUserIsNotAuthorized_DoNotShowAgreement()
        {
            var latestAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 1 },
                StatusId = EmployerAgreementStatus.Pending
            };

            return RunAsync(
                arrange: fixtures => fixtures.WithAgreement(latestAgreement)
                                             .WithUnauthorizedUser(),
                act: fixtures => fixtures.Handle(),
                assert: fixtures =>
                    Assert.IsInstanceOf<UnauthorizedAccessException>(fixtures.Exception));
        }

        [Test]
        public Task GetAgreementToSign_IfRequestIsNotValid_DoNotShowAgreement()
        {
            var latestAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 1 },
                StatusId = EmployerAgreementStatus.Pending
            };

            return RunAsync(
                arrange: fixtures => fixtures.WithAgreement(latestAgreement)
                                             .WithInvalidRequest(),
                act: fixtures => fixtures.Handle(),
                assert: fixtures =>
                    Assert.IsInstanceOf<InvalidRequestException>(fixtures.Exception));
        }

        [Test]
        public Task GetAgreementToSign_IfNoAgreementFound_ShouldReturn()
        {
            return RunAsync(
                act: fixtures => fixtures.Handle(),
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
            var latestAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId + 1,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 2 },
                StatusId = EmployerAgreementStatus.Pending
            };

            var expectedAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 1 },
                StatusId = EmployerAgreementStatus.Pending
            };

            return RunAsync(
                arrange: fixtures => fixtures.WithAgreement(latestAgreement)
                                             .WithAgreement(expectedAgreement),
                act: fixtures => fixtures.Handle(),
                assert: fixtures => expectedAgreement.Should().NotBeNull().And
                                                     .Match<EmployerAgreement>(a =>
                        a.AccountId == fixtures.Response.EmployerAgreement.AccountId &&
                        a.LegalEntityId == fixtures.Response.EmployerAgreement.LegalEntityId &&
                        a.Template.VersionNumber.Equals(fixtures.Response.EmployerAgreement.Template.VersionNumber) &&
                        a.StatusId.Equals(fixtures.Response.EmployerAgreement.StatusId)));
        }

        [Test]
        public Task GetAgreementToSign_ShouldReturnLatestSignedAgreement()
        {
            var latestAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 3 },
                StatusId = EmployerAgreementStatus.Pending
            };

            var signedAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId - 1,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 2 },
                SignedByName = "Test User",
                SignedDate = DateTime.Now.AddDays(-30),
                StatusId = EmployerAgreementStatus.Signed
            };

            var olderSignedAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId - 2,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 1 },
                SignedByName = "Test User",
                SignedDate = DateTime.Now.AddDays(-60),
                StatusId = EmployerAgreementStatus.Signed
            };

            return RunAsync(
                arrange: fixtures => fixtures.WithAgreement(latestAgreement)
                    .WithAgreement(signedAgreement)
                    .WithAgreement(olderSignedAgreement),
                act: fixtures => fixtures.Handle(),
                assert: fixtures => signedAgreement.Should().NotBeNull()
                    .And.Match<EmployerAgreement>(a =>
                        a.AccountId == fixtures.Response.LastSignedAgreement.AccountId &&
                        a.LegalEntityId == fixtures.Response.LastSignedAgreement.LegalEntityId &&
                        a.SignedByName.Equals(fixtures.Response.LastSignedAgreement.SignedByName) &&
                        a.SignedDate.Equals(fixtures.Response.LastSignedAgreement.SignedDate) &&
                        a.Template.VersionNumber.Equals(fixtures.Response.LastSignedAgreement.Template.VersionNumber) &&
                        a.StatusId.Equals(fixtures.Response.LastSignedAgreement.StatusId)));
        }

        [Test]
        public Task GetAgreementToSign_IfNoSignedAgreementsExists_ShouldReturnNoSignedAgreement()
        {
            var latestAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 3 },
                StatusId = EmployerAgreementStatus.Pending
            };

            return RunAsync(
                arrange: fixtures => fixtures.WithAgreement(latestAgreement),
                act: fixtures => fixtures.Handle(),
                assert: fixtures =>
                    Assert.IsNull(fixtures.Response.LastSignedAgreement));
        }

        [Test]
        public Task GetAgreementToSign_IfSignedAgreementIfForAnotherAccount_ShouldNotReturnSignedAgreement()
        {
            var latestAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 3 },
                StatusId = EmployerAgreementStatus.Pending
            };

            var signedAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId - 1,
                AccountId = GetEmployerAgreementTestFixtures.AccountId + 1,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 2 },
                SignedByName = "Test User",
                SignedDate = DateTime.Now.AddDays(-30),
                StatusId = EmployerAgreementStatus.Signed
            };


            return RunAsync(
                arrange: fixtures => fixtures.WithAgreement(latestAgreement)
                                             .WithAgreement(signedAgreement),
                act: fixtures => fixtures.Handle(),
                assert: fixtures =>
                    Assert.IsNull(fixtures.Response.LastSignedAgreement));
        }

        [Test]
        public Task GetAgreementToSign_IfRequestAgreementIsSigned_ShouldNotReturnLastestSignedAgreementAsWell()
        {
            var latestAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 3 },
                StatusId = EmployerAgreementStatus.Signed
            };

            var signedAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId - 1,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                Template = new AgreementTemplate { VersionNumber = 2 },
                SignedByName = "Test User",
                SignedDate = DateTime.Now.AddDays(-30),
                StatusId = EmployerAgreementStatus.Signed
            };

            return RunAsync(
                arrange: fixtures => fixtures.WithAgreement(latestAgreement)
                                             .WithAgreement(signedAgreement),
                act: fixtures => fixtures.Handle(),
                assert: fixtures =>
                    Assert.IsNull(fixtures.Response.LastSignedAgreement));
        }

        [Test]
        public Task GetAgreementToSign_IfAgreementHasNotBeenSigned_ShouldAddCurrentUserAsSigner()
        {
            var latestAgreement = new EmployerAgreement
            {
                Id = GetEmployerAgreementTestFixtures.AgreementId,
                AccountId = GetEmployerAgreementTestFixtures.AccountId,
                LegalEntityId = GetEmployerAgreementTestFixtures.LegalEntityId,
                TemplateId = 5,
                Template = new AgreementTemplate { Id = 5, VersionNumber = 2 },
                StatusId = EmployerAgreementStatus.Pending
            };

            return RunAsync(
                arrange: fixtures => fixtures.WithAgreement(latestAgreement),
                act: fixtures => fixtures.Handle(),
                assert: fixtures => Assert.AreEqual(fixtures.CurrentUser.FullName, fixtures.Response.EmployerAgreement.SignedByName));
        }
    }

    internal class GetEmployerAgreementTestFixtures : FluentTestFixture
    {
        public const string HashedAccountId = "SDF768";
        public const string HashedAgreementId = "GHD567";
        public const long LegalEntityId = 54;
        public const long AccountId = 2;
        public const long AgreementId = 20;


        public Mock<EmployerAccountDbContext> Database { get; }
        public Mock<IHashingService> HashingService { get; }
        public Mock<IValidator<GetEmployerAgreementRequest>> Validator { get; }
        public IConfigurationProvider ConfigurationProvider { get; }

        public GetEmployerAgreementQueryHandler Handler { get; }
        public GetEmployerAgreementRequest Request { get; }
        public GetEmployerAgreementResponse Response { get; private set; }

        public ICollection<EmployerAgreement> Agreements { get; set; }
        public ICollection<Membership> Memberships { get; private set; }
        public ICollection<User> Users { get; private set; }
        public User CurrentUser { get; private set; }

        public Exception Exception { get; private set; }


        public GetEmployerAgreementTestFixtures()
        {
            Database = new Mock<EmployerAccountDbContext>();
            HashingService = new Mock<IHashingService>();
            Validator = new Mock<IValidator<GetEmployerAgreementRequest>>();

            Agreements = new List<EmployerAgreement>();

            CreateUserAccountMembership();

            ConfigurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<EmploymentAgreementStatusMappings>();
                c.AddProfile<AgreementMappings>();

            });

            Handler = new GetEmployerAgreementQueryHandler(Database.Object, HashingService.Object, Validator.Object, ConfigurationProvider);
            Request = new GetEmployerAgreementRequest
            {
                HashedAccountId = HashedAccountId,
                AgreementId = HashedAgreementId,
                ExternalUserId = CurrentUser.ExternalId.ToString()
            };

            HashingService.Setup(x => x.DecodeValue(HashedAccountId)).Returns(AccountId);
            HashingService.Setup(x => x.DecodeValue(HashedAgreementId)).Returns(AgreementId);

            Validator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAgreementRequest>()))
                     .ReturnsAsync(new ValidationResult());
        }

        public async Task Handle()
        {
            MockDbContextDbSets();

            try
            {
                Response = await Handler.Handle(Request);
            }
            catch (Exception e)
            {
                Exception = e;
            }
        }

        public GetEmployerAgreementTestFixtures WithAgreement(EmployerAgreement agreement)
        {
            Agreements.Add(agreement);

            return this;
        }

        public GetEmployerAgreementTestFixtures WithUnauthorizedUser()
        {
            Validator.Setup(x => x.ValidateAsync(Request))
                .ReturnsAsync(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>(),
                    IsUnauthorized = true
                });

            return this;
        }

        public GetEmployerAgreementTestFixtures WithInvalidRequest()
        {
            Validator.Setup(x => x.ValidateAsync(Request))
                .ReturnsAsync(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        {nameof(Request.HashedAccountId), "Account Id is not a valid Id"}
                    },
                    IsUnauthorized = false
                });

            return this;
        }

        private void CreateUserAccountMembership()
        {
            CurrentUser = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 2,
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "Person"
            };

            Users = new List<User>
            {
                CurrentUser
            };

            Memberships = new List<Membership>
            {
                new Membership
                {
                    UserId = CurrentUser.Id,
                    User = CurrentUser,
                    AccountId = AccountId,
                    Account = new Domain.Models.Account.Account {Id = AccountId}
                }
            };
        }

        private void MockDbContextDbSets()
        {
            Database.Setup(x => x.Agreements)
                    .Returns(new DbSetStub<EmployerAgreement>(Agreements.AsEnumerable()));

            Database.Setup(x => x.Memberships)
                .Returns(new DbSetStub<Membership>(Memberships.AsEnumerable()));

            Database.Setup(x => x.Users)
                .Returns(new DbSetStub<User>(Users.AsEnumerable()));
        }

    }
}
