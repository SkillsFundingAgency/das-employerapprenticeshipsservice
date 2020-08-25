﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetLegalEntity;
using SFA.DAS.EmployerAccounts.TestCommon;
using FluentTestFixture = SFA.DAS.Testing.FluentTestFixture;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetLegalEntityQueryTests
{
    [TestFixture]
    public class GetLegalEntityQueryTests : Testing.FluentTest<GetLegalEntityQueryTestsFixture>
    {
        [Test]
        public Task Handle_WhenGettingLegalEntity_ThenShouldReturnLegalEntity()
        {
            return RunAsync(f => f.Handle(), (f, r) => r.Should().NotBeNull()
                .And.Match<GetLegalEntityResponse>(r2 => 
                    r2.LegalEntity.LegalEntityId == f.LegalEntity.Id &&
                    r2.LegalEntity.Agreements.Count == 3 &&
                    r2.LegalEntity.Agreements.Any(a => a.TemplateVersionNumber == 1 && a.Status == Api.Types.EmployerAgreementStatus.Signed) &&
                    r2.LegalEntity.Agreements.Any(a => a.TemplateVersionNumber == 2 && a.Status == Api.Types.EmployerAgreementStatus.Signed) &&
                    r2.LegalEntity.Agreements.Any(a => a.TemplateVersionNumber == 3 && a.Status == Api.Types.EmployerAgreementStatus.Pending) &&
                    r2.LegalEntity.AgreementStatus == Api.Types.EmployerAgreementStatus.Pending &&
                    r2.LegalEntity.Agreements.Any(a => a.AgreementType == f.LegalEntity.AccountLegalEntities.First().Agreements.First().Template.AgreementType)
                ));
        }

        [Test]
        public async Task Handle_WhenGettingLegalEntity_ThenShouldReturnTheCorrectEmailOfUserWhoSignedTheAgreement()
        {
            var f = new GetLegalEntityQueryTestsFixture();
            var result = await f.Handle();

            result.LegalEntity.Agreements.First(a => a.TemplateVersionNumber == 1 && a.Status == Api.Types.EmployerAgreementStatus.Signed).SignedByEmail.Should().Be(f.UserB.Email);
            result.LegalEntity.Agreements.First(a => a.TemplateVersionNumber == 2 && a.Status == Api.Types.EmployerAgreementStatus.Signed).SignedByEmail.Should().Be(f.UserA.Email);
        }
    }

    public class GetLegalEntityQueryTestsFixture : FluentTestFixture
    {
        public GetLegalEntityQueryHandler Handler { get; set; }
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public IConfigurationProvider ConfigurationProvider { get; set; }
        public Account Account { get; private set; }
        public LegalEntity LegalEntity { get; set; }
        public AccountLegalEntity AccountLegalEntity { get; set; }
        public DbSetStub<LegalEntity> LegalEntitiesDbSet { get; set; }
        public List<LegalEntity> LegalEntities { get; set; }
        public List<AccountLegalEntity> AccountLegalEntities { get; set; }
        public DbSetStub<AccountLegalEntity> AccountLegalEntitiesDbSet { get; set; }
        public User UserA { get; set; }
        public User UserB { get; set; }
        public List<User> Users { get; set; }
        public DbSetStub<User> UsersDbSet { get; set; }

        public GetLegalEntityQueryTestsFixture()
        {
            Db = new Mock<EmployerAccountsDbContext>();

            ConfigurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AgreementMappings>();
                c.AddProfile<LegalEntityMappings>();
            });

            LegalEntities = new List<LegalEntity>();
            LegalEntitiesDbSet = new DbSetStub<LegalEntity>(LegalEntities);

            AccountLegalEntities = new List<AccountLegalEntity>();
            AccountLegalEntitiesDbSet = new DbSetStub<AccountLegalEntity>(AccountLegalEntities);

            UserA = new User { Id = 1, Email = "UserA@gmail.com"};
            UserB = new User { Id = 2, Email = "UserB@gmail.com"};
            Users = new List<User> { UserA, UserB };
            UsersDbSet = new DbSetStub<User>(Users);

            Db.Setup(d => d.LegalEntities).Returns(LegalEntitiesDbSet);
            Db.Setup(d => d.AccountLegalEntities).Returns(AccountLegalEntitiesDbSet);
            Db.Setup(d => d.Users).Returns(UsersDbSet);

            Handler = new GetLegalEntityQueryHandler(new Lazy<EmployerAccountsDbContext>(() => Db.Object), ConfigurationProvider);

            SetAccount()
                .SetLegalEntity()
                .SetLegalAccountLegalEntity()
                .AddLegalEntityAgreement(1, EmployerAccounts.Models.EmployerAgreement.EmployerAgreementStatus.Removed, UserA.Id)
                .AddLegalEntityAgreement(1, EmployerAccounts.Models.EmployerAgreement.EmployerAgreementStatus.Signed, UserB.Id)
                .AddLegalEntityAgreement(2, EmployerAccounts.Models.EmployerAgreement.EmployerAgreementStatus.Signed, UserA.Id)
                .AddLegalEntityAgreement(3, EmployerAccounts.Models.EmployerAgreement.EmployerAgreementStatus.Pending)
                .EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities();
        }

        public Task<GetLegalEntityResponse> Handle()
        {
            return Handler.Handle(
                new GetLegalEntityQuery(
                    Account.HashedId,
                    LegalEntity.Id
                ));
        }

        public GetLegalEntityQueryTestsFixture SetAccount()
        {
            Account = new Account
            {
                Id = 111111,
                HashedId = "ABC123"
            };

            return this;
        }

        public GetLegalEntityQueryTestsFixture EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities()
        {
            EmployerAgreement FindVersionToUse(AccountLegalEntity ale, EmployerAccounts.Models.EmployerAgreement.EmployerAgreementStatus status)
            {
                return ale.Agreements.Where(a => a.StatusId == status)
                    .OrderByDescending(a => a.Template.VersionNumber)
                    .FirstOrDefault();
            }

            foreach (var accountLegalEntity in AccountLegalEntities)
            {
                var pending = FindVersionToUse(accountLegalEntity, EmployerAccounts.Models.EmployerAgreement.EmployerAgreementStatus.Pending);
                var signed = FindVersionToUse(accountLegalEntity, EmployerAccounts.Models.EmployerAgreement.EmployerAgreementStatus.Signed);
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

        private GetLegalEntityQueryTestsFixture AddLegalEntityAgreement(int versionNumber, EmployerAccounts.Models.EmployerAgreement.EmployerAgreementStatus status, long? signedUserId = null)
        {
            var newAgreement = new EmployerAgreement
            {
                Template = new AgreementTemplate
                {
                    VersionNumber = versionNumber,
                    AgreementType = AgreementType.Levy
                },
                StatusId = status,
                SignedById = signedUserId
            };

            newAgreement.AccountLegalEntity = AccountLegalEntity;
            newAgreement.AccountLegalEntityId = AccountLegalEntity.Id;
            AccountLegalEntity.Agreements.Add(newAgreement);

            return this;
        }
    }
}