using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Util;
using MediatR;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Application.Queries.GetApprovedTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.HashingService;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountEmployerAgreementTests
{
    [TestFixture]
    public class GetAccountEmployerAgreementTests
    {
        [Test]
        public async Task Handle_WithAnyRequest_ShouldCallValidator()
        {
            // Arrange
            var fixtures = new GetAccountEmployerAgreementTestFixtures()
                                .WithRequestedAccount(123, "ABC123");
            var request = fixtures.CreateRequest();
            var queryHandler = fixtures.CreateQueryHandler();

            //Act
            await queryHandler.Handle(request);

            //Assert
            fixtures.ValidatorMock.Verify(v => v.Validate(request), Times.Once);
        }

        [Test]
        public void Handle_WithUnauthorisedRequest_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var fixtures = new GetAccountEmployerAgreementTestFixtures()
                                    .WithUnauthorisedRequest()
                                    .WithRequestedAccount(123, "ABC123");
            var request = fixtures.CreateRequest();

            var queryHandler = fixtures.CreateQueryHandler();

            //Act
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await queryHandler.Handle(request));
        }

        [Test]
        public async Task Handle_WithAccountThatHasOneSignedAgreement_ShouldReturnSignedAgreement()
        {
            //Act
            var fixtures = new GetAccountEmployerAgreementTestFixtures()
                .WithUnauthorisedRequest()
                .WithRequestedAccount(123, "ABC123");

            var request = fixtures.CreateRequest();
            var queryHandler = fixtures.CreateQueryHandler();

            //Act
            var response = await queryHandler.Handle(request);

            //Assert
            Assert.IsNotNull(response);
        }
    }

    class GetAccountEmployerAgreementTestFixtures
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

        public GetAccountEmployerAgreementTestFixtures WithUnauthorisedRequest()
        {
            ValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<GetAccountEmployerAgreementsRequest>()))
                .ReturnsAsync(new ValidationResult { IsUnauthorized = true });

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

        public GetAccountEmployerAgreementTestFixtures WithLegalEntityId(long legalEntityId, string name)
        {
            LegalEntities.Add(new LegalEntity
            {
                Id = legalEntityId,
                Name = name
            });

            return this;
        }

        public GetAccountEmployerAgreementTestFixtures WithAgreement(long accountId, long legalEntityId, int agreementVersion)
        {
            var account = Accounts.Single(ac => ac.Id == accountId);
            var legalEntity = LegalEntities.Single(le => le.Id == legalEntityId);
            var template = AgreementTemplates.Single(ag => ag.VersionNumber == agreementVersion);

            EmployerAgreements.Add(new EmployerAgreement
            {
                Id = accountId,
                Account = account,
                LegalEntity = legalEntity,
                Template = template,
            });

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

            EmployerAccountDbContextMock.Setup(db => db.Accounts).Returns(accountsDbSet);
            EmployerAccountDbContextMock.Setup(db => db.Agreements).Returns(agreementsDbSet);

            var queryHandler = new GetAccountEmployerAgreementsQueryHandler(EmployerAccountDbContext, HashingService, Validator);

            return queryHandler;
        }
    }
}
