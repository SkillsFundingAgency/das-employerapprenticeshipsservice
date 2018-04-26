using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

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
                assert: fixtures =>
                    Assert.AreEqual(expectedAgreement, fixtures.Response.EmployerAgreement));
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
                assert: fixtures =>
                    Assert.AreEqual(signedAgreement, fixtures.Response.LastSignedAgreement));
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
    }

    internal class GetEmployerAgreementTestFixtures : FluentTestFixture
    {
        public const string HashedAccountId = "SDF768";
        public const string HashedAgreementId = "GHD567";
        public const long LegalEntityId = 54;
        public const long AccountId = 2;
        public const long AgreementId = 20;
        public const string UserId = "ABC123";

        public Mock<EmployerAccountDbContext> Database { get; }
        public Mock<IHashingService> HashingService { get; }
        public Mock<IValidator<GetEmployerAgreementRequest>> Validator { get; }
        public GetEmployerAgreementQueryHandler Handler { get; }
        public GetEmployerAgreementRequest Request { get; }
        public GetEmployerAgreementResponse Response { get; private set; }
        public ICollection<EmployerAgreement> Agreements { get; set; }
        public Exception Exception { get; private set; }

        public GetEmployerAgreementTestFixtures()
        {
            Database = new Mock<EmployerAccountDbContext>();
            HashingService = new Mock<IHashingService>();
            Validator = new Mock<IValidator<GetEmployerAgreementRequest>>();
            Handler = new GetEmployerAgreementQueryHandler(Database.Object, HashingService.Object, Validator.Object);
            Request = new GetEmployerAgreementRequest
            {
                HashedAccountId = HashedAccountId,
                HashedAgreementId = HashedAgreementId,
                ExternalUserId = UserId
            };

            Agreements = new List<EmployerAgreement>();

            HashingService.Setup(x => x.DecodeValue(HashedAccountId)).Returns(AccountId);
            HashingService.Setup(x => x.DecodeValue(HashedAgreementId)).Returns(AgreementId);

            Validator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAgreementRequest>()))
                     .ReturnsAsync(new ValidationResult());
        }

        public async Task Handle()
        {
            MockAgreementDbSet();

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


        private void MockAgreementDbSet()
        {
            var agreementData = Agreements.AsQueryable();

            var mockSet = new Mock<DbSet<EmployerAgreement>>();
            mockSet.As<IQueryable<EmployerAgreement>>().Setup(m => m.Provider).Returns(agreementData.Provider);
            mockSet.As<IQueryable<EmployerAgreement>>().Setup(m => m.Expression).Returns(agreementData.Expression);
            mockSet.As<IQueryable<EmployerAgreement>>().Setup(m => m.ElementType).Returns(agreementData.ElementType);
            mockSet.As<IQueryable<EmployerAgreement>>().Setup(m => m.GetEnumerator()).Returns(() => agreementData.GetEnumerator());

            Database.Setup(x => x.Agreements).Returns(mockSet.Object);
        }
    }
}
