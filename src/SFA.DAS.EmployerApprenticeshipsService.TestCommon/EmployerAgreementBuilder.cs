using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.TestCommon
{
    /// <summary>
    ///     Helper class for tests to build up EmployerAgreements, Accounts, LegalEntities and AccountLegalEntities
    ///     and have the relationships between them maintained so that the mocked DBContexts work as expected.
    /// </summary>
    /// <remarks>
    ///     This would normally be in a test fixtures class but there is quite a bit of setup logic going on here
    ///     and it's used in several places.
    /// </remarks>
    public class EmployerAgreementBuilder
    {
        public EmployerAgreementBuilder()
        {
            HashingServiceMock = new Mock<IHashingService>();
            Accounts = new List<Domain.Models.Account.Account>();
            AccountLegalEntities = new List<AccountLegalEntity>();
            AgreementTemplates = new List<AgreementTemplate>();
            EmployerAgreements = new List<EmployerAgreement>();
            Users = new List<User>();
            Memberships = new List<Membership>();
            LegalEntities = new List<LegalEntity>();
            EmployerAccountDbContextMock = new Mock<EmployerAccountsDbContext>();
        }

        public Mock<IHashingService> HashingServiceMock { get; }
        public IHashingService HashingService => HashingServiceMock.Object;
        public Mock<IHashingService> AccountLegalEntityHashingServiceMock { get; }
        public IHashingService AccountLegalEntityHashingService => AccountLegalEntityHashingServiceMock.Object;
        public List<Domain.Models.Account.Account> Accounts { get; }
        public List<AgreementTemplate> AgreementTemplates { get; set; }
        public List<AccountLegalEntity> AccountLegalEntities { get; set; }
        public List<EmployerAgreement> EmployerAgreements { get; }
        public List<LegalEntity> LegalEntities { get; }
        public List<User> Users { get; }
        public List<Membership> Memberships { get; }

        public EmployerAgreementBuilder WithAccount(long accountId, string hashedId)
        {
            Accounts.Add(new Domain.Models.Account.Account
            {
                Id =  accountId,
            });

            AddHash(accountId, hashedId);

            return this;
        }


        public EmployerAgreementBuilder WithLegalEntityId(long legalEntityId)
        {
            LegalEntities.Add(new LegalEntity
            {
                Id = legalEntityId
            });

            return this;
        }

        public EmployerAgreementBuilder WithAgreement(EmployerAgreement employerAgreement, long accountId, long legalEntityId)
        {
            var accountLegalEntity = EnsureAccountLegalEntity(accountId, legalEntityId);
            return WithAgreement(employerAgreement, accountLegalEntity);
        }

        public EmployerAgreementBuilder WithSignedAgreement(long accountId, long legalEntityId, int agreementVersion, DateTime signeDateTime, out EmployerAgreement employerAgreement)
        {
            WithAgreement(accountId, legalEntityId, agreementVersion, EmployerAgreementStatus.Signed, out employerAgreement);
            employerAgreement.SignedDate = signeDateTime;
            return this;
        }

        public EmployerAgreementBuilder WithPendingAgreement(long accountId, long legalEntityId, int agreementVersion)
        {
            return WithAgreement(accountId, legalEntityId, agreementVersion, EmployerAgreementStatus.Pending);
        }

        public EmployerAgreementBuilder WithPendingAgreement(long accountId, long legalEntityId, int agreementVersion, out EmployerAgreement employerAgreement)
        {
            return WithAgreement(accountId, legalEntityId, agreementVersion, EmployerAgreementStatus.Pending, out employerAgreement);
        }

        public EmployerAgreementBuilder WithAgreement(long accountId, long legalEntityId, int agreementVersion, EmployerAgreementStatus status, out EmployerAgreement employerAgreement)
        {
            var template = EnsureTemplate(agreementVersion);
            var accountLegalEntity = EnsureAccountLegalEntity(accountId, legalEntityId);

            employerAgreement = new EmployerAgreement
            {
                Id = EmployerAgreements.Count + 1000, // offset from account Ids so that hashing mock won't get clashing ids
                Template = template,
                TemplateId = template.Id,
                StatusId = status
            };

            template.Agreements.Add(employerAgreement);
            
            return WithAgreement(employerAgreement, accountLegalEntity);
        }

        public EmployerAgreementBuilder WithAgreement(EmployerAgreement employerAgreement, AccountLegalEntity accountLegalEntity)
        {
            employerAgreement.AccountLegalEntity = accountLegalEntity;
            employerAgreement.AccountLegalEntityId = accountLegalEntity.Id;

            EmployerAgreements.Add(employerAgreement);

            accountLegalEntity.Agreements.Add(employerAgreement);

            var  agreementHash = $"AGR{employerAgreement.Id:D3}";
            
            AddHash(employerAgreement.Id, agreementHash);
            return this;
        }

        public EmployerAgreementBuilder WithAgreement(long accountId, long legalEntityId, int agreementVersion, EmployerAgreementStatus status)
        {
            return WithAgreement(accountId, legalEntityId, agreementVersion, status, out _);
        }

        public EmployerAgreementBuilder RemoveAccountLegalEntity(long accountId, long legalEntityId)
        {
            var accountLegalEntity = EnsureAccountLegalEntity(accountId, legalEntityId);
            accountLegalEntity.Deleted = DateTime.Now;
            return this;
        }

        public EmployerAgreementBuilder EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities()
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

        public Mock<EmployerAccountsDbContext> EmployerAccountDbContextMock { get; set; }

        public EmployerAccountsDbContext EmployerAccountDbContext => EmployerAccountDbContextMock.Object;

        public EmployerAgreementBuilder SetupMockDbContext()
        {
            DbSetStub<Domain.Models.Account.Account> accountsDbSet = new DbSetStub<Domain.Models.Account.Account>(Accounts);
            DbSetStub<EmployerAgreement> agreementsDbSet = new DbSetStub<EmployerAgreement>(EmployerAgreements);
            DbSetStub<AccountLegalEntity> accountLegalEntityDbSet = new DbSetStub<AccountLegalEntity>(AccountLegalEntities);
            DbSetStub<AgreementTemplate> agreementTemplateEntityDbSet = new DbSetStub<AgreementTemplate>(AgreementTemplates);
            DbSetStub<User> userEntityDbSet = new DbSetStub<User>(Users);
            DbSetStub<Membership> membershipEntityDbSet = new DbSetStub<Membership>(Memberships);

            EmployerAccountDbContextMock
                .Setup(db => db.Accounts)
                .Returns(accountsDbSet);

            EmployerAccountDbContextMock
                .Setup(db => db.Agreements)
                .Returns(agreementsDbSet);

            EmployerAccountDbContextMock
                .Setup(db => db.AccountLegalEntities)
                .Returns(accountLegalEntityDbSet);

            EmployerAccountDbContextMock
                .Setup(db => db.AgreementTemplates)
                .Returns(agreementTemplateEntityDbSet);

            EmployerAccountDbContextMock
                .Setup(db => db.Users)
                .Returns(userEntityDbSet);

            EmployerAccountDbContextMock
                .Setup(db => db.Memberships)
                .Returns(membershipEntityDbSet);

            return this;
        }

        public EmployerAgreementBuilder WithUser(long accountId, string firstName, string lastName, out User user)
        {
            var account = GetAccount(accountId);

            user = new User
            {
                Ref = Guid.NewGuid(),
                Id = Users.Count+1,
                Email = $"{firstName}.{lastName}@test.com",
                FirstName = firstName,
                LastName = lastName
            };

            Users.Add(user);

            var membership = new Membership
            {
                UserId = user.Id,
                User = user,
                AccountId = account.Id,
                Account = account
            };

            Memberships.Add(membership);

            return this;
        }

        private AgreementTemplate EnsureTemplate(int agreementVersion)
        {
            var template = AgreementTemplates.FirstOrDefault(ag => ag.VersionNumber == agreementVersion);
            if (template == null)
            {
                AgreementTemplates.Add(template = new AgreementTemplate { VersionNumber = agreementVersion });
            }

            return template;
        }

        private LegalEntity EnsureLegalEntity(long legalEntityId)
        {
            var legalEngtity = LegalEntities.FirstOrDefault(le => le.Id == legalEntityId);

            if (legalEngtity == null)
            {
                LegalEntities.Add(legalEngtity = new LegalEntity { Id = legalEntityId });
            }

            return legalEngtity;
        }

        public Domain.Models.Account.Account GetAccount(long accountId)
        {
            var account = Accounts.FirstOrDefault(a => a.Id == accountId);

            if (account == null)
            {
                Assert.Fail($"The test setup is attempting to use account {accountId} before it has been setup");
            }

            return account;
        }

        private void AddHash(long id, string hashValue)
        {
            HashingServiceMock.Setup(c => c.DecodeValue(hashValue)).Returns(id);
            HashingServiceMock.Setup(c => c.HashValue(id)).Returns(hashValue);
        }

        private AccountLegalEntity EnsureAccountLegalEntity(long accountId, long legalEntityId)
        {
            var accountLegalEntity = AccountLegalEntities.FirstOrDefault(ale => ale.AccountId == accountId && ale.LegalEntityId == legalEntityId);

            if (accountLegalEntity == null)
            {
                var legalEntity = EnsureLegalEntity(legalEntityId);
                var account = GetAccount(accountId);

                AccountLegalEntities.Add(accountLegalEntity = new AccountLegalEntity
                {
                    AccountId = accountId,
                    LegalEntityId = legalEntityId,
                    Account = account,
                    LegalEntity = legalEntity
                });

                account.AccountLegalEntities.Add(accountLegalEntity);
                legalEntity.AccountLegalEntities.Add(accountLegalEntity);
            }

            return accountLegalEntity;
        }
    }
}