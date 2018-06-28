using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetLegalEntity;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLegalEntityQueryTests
{
    [TestFixture]
    public class GetLegalEntityQueryTests : FluentTest<GetLegalEntityQueryTestsFixture>
    {
        [Test]
        public Task Handle_WhenGettingLegalEntity_ThenShouldReturnLegalEntity()
        {
            return RunAsync(f => f.Handle(), (f, r) => r.Should().NotBeNull()
                .And.Match<GetLegalEntityResponse>(r2 => 
                    r2.LegalEntity.LegalEntityId == f.LegalEntity.Id &&
                    r2.LegalEntity.Agreements.Count == 2 &&
                    r2.LegalEntity.Agreements.Any(a => a.TemplateVersionNumber == 1 && a.Status == Account.Api.Types.EmployerAgreementStatus.Signed) &&
                    r2.LegalEntity.Agreements.Any(a => a.TemplateVersionNumber == 2 && a.Status == Account.Api.Types.EmployerAgreementStatus.Pending) &&
                    r2.LegalEntity.AgreementStatus == Account.Api.Types.EmployerAgreementStatus.Pending));
        }
    }

    public class GetLegalEntityQueryTestsFixture : FluentTestFixture
    {
        public GetLegalEntityQueryHandler Handler { get; set; }
        public Mock<EmployerAccountDbContext> Db { get; set; }
        public IConfigurationProvider ConfigurationProvider { get; set; }
        public Domain.Models.Account.Account Account { get; private set; }
        public LegalEntity LegalEntity { get; set; }
        public AccountLegalEntity AccountLegalEntity { get; set; }
        public DbSetStub<LegalEntity> LegalEntitiesDbSet { get; set; }
        public List<LegalEntity> LegalEntities { get; set; }
        public List<AccountLegalEntity> AccountLegalEntities { get; set; }
        public DbSetStub<AccountLegalEntity> AccountLegalEntitiesDbSet { get; set; }

        public GetLegalEntityQueryTestsFixture()
        {
            Db = new Mock<EmployerAccountDbContext>();

            ConfigurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AgreementMappings>();
                c.AddProfile<LegalEntityMappings>();
            });

            LegalEntities = new List<LegalEntity>();
            LegalEntitiesDbSet = new DbSetStub<LegalEntity>(LegalEntities);

            AccountLegalEntities = new List<AccountLegalEntity>();
            AccountLegalEntitiesDbSet = new DbSetStub<AccountLegalEntity>(AccountLegalEntities);

            Db.Setup(d => d.LegalEntities).Returns(LegalEntitiesDbSet);
            Db.Setup(d => d.AccountLegalEntities).Returns(AccountLegalEntitiesDbSet);

            Handler = new GetLegalEntityQueryHandler(new Lazy<EmployerAccountDbContext>(() => Db.Object), ConfigurationProvider);

            SetAccount()
                .SetLegalEntity()
                .SetLegalAccountLegalEntity()
                .AddLegalEntityAgreement(1, EmployerAgreementStatus.Removed)
                .AddLegalEntityAgreement(1, EmployerAgreementStatus.Signed)
                .AddLegalEntityAgreement(2, EmployerAgreementStatus.Pending)
                .EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities();
        }

        public Task<GetLegalEntityResponse> Handle()
        {
            return Handler.Handle(new GetLegalEntityQuery
            {
                AccountId = Account.Id,
                AccountHashedId = Account.HashedId,
                LegalEntityId = LegalEntity.Id
            });
        }

        public GetLegalEntityQueryTestsFixture SetAccount()
        {
            Account = new Domain.Models.Account.Account
            {
                Id = 111111,
                HashedId = "ABC123"
            };

            return this;
        }

        public GetLegalEntityQueryTestsFixture EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities()
        {
            EmployerAgreement FindVersionToUse(AccountLegalEntity ale, EmployerAgreementStatus status)
            {
                return ale.Agreements.Where(a => a.StatusId == status)
                    .OrderByDescending(a => a.Template.VersionNumber)
                    .FirstOrDefault();
            }

            foreach (var accountLegalEntity in AccountLegalEntities)
            {
                var pending = FindVersionToUse(accountLegalEntity, EmployerAgreementStatus.Pending);
                var signed = FindVersionToUse(accountLegalEntity, EmployerAgreementStatus.Signed);
                accountLegalEntity.PendingAgreementId = pending?.Id;
                accountLegalEntity.PendingAgreement = pending;
                accountLegalEntity.PendingAgreementVersion = pending?.Template?.VersionNumber;

                accountLegalEntity.SignedAgreementId = signed?.Id;
                accountLegalEntity.SignedAgreement = signed;
                accountLegalEntity.SignedAgreementVersion = signed?.Template?.VersionNumber;
            }

            return this;
        }

        private GetLegalEntityQueryTestsFixture SetLegalEntity()
        {
            LegalEntity = new LegalEntity
            {
                Id = 222222
            };

            LegalEntities.Add(LegalEntity);
            return this;
        }

        private GetLegalEntityQueryTestsFixture SetLegalAccountLegalEntity()
        {

            AccountLegalEntity = new AccountLegalEntity
            {
                Id = AccountLegalEntities.Count + 1,
                Account = Account,
                AccountId = Account.Id,
                LegalEntity = LegalEntity,
                LegalEntityId = LegalEntity.Id
            };

            LegalEntity.AccountLegalEntities.Add(AccountLegalEntity);
            Account.AccountLegalEntities.Add(AccountLegalEntity);

            AccountLegalEntities.Add(AccountLegalEntity);

            return this;
        }

        private GetLegalEntityQueryTestsFixture AddLegalEntityAgreement(int versionNumber, EmployerAgreementStatus status)
        {
            var newAgreement = new EmployerAgreement
            {
                Template = new AgreementTemplate
                {
                    VersionNumber = versionNumber
                },
                StatusId = status
            };

            newAgreement.AccountLegalEntity = AccountLegalEntity;
            newAgreement.AccountLegalEntityId = AccountLegalEntity.Id;
            AccountLegalEntity.Agreements.Add(newAgreement);

            return this;
        }
    }
}