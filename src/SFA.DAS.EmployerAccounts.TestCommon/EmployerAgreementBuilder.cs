using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.TestCommon;

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
        EncodingServiceMock = new Mock<IEncodingService>();
        Accounts = new List<Account>();
        AccountLegalEntities = new List<AccountLegalEntity>();
        AgreementTemplates = new List<AgreementTemplate>();
        EmployerAgreements = new List<EmployerAgreement>();
        Users = new List<User>();
        Memberships = new List<Membership>();
        LegalEntities = new List<LegalEntity>();
        EmployerAccountDbContextMock = new Mock<EmployerAccountsDbContext>();
    }

    public Mock<IEncodingService> EncodingServiceMock { get; }
    public IEncodingService EncodingService => EncodingServiceMock.Object;
    public List<Account> Accounts { get; }
    public List<AgreementTemplate> AgreementTemplates { get; set; }
    public List<AccountLegalEntity> AccountLegalEntities { get; set; }
    public List<EmployerAgreement> EmployerAgreements { get; }
    public List<LegalEntity> LegalEntities { get; }
    public List<User> Users { get; }
    public List<Membership> Memberships { get; }

    public EmployerAgreementBuilder WithAccount(long accountId, string hashedId)
    {
        Accounts.Add(new Account
        {
            Id = accountId,
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

    public EmployerAgreementBuilder WithAgreement(EmployerAgreement employerAgreement, long accountId, long legalEntityId, long accountLegalEntityId)
    {
        var accountLegalEntity = EnsureAccountLegalEntity(accountId, legalEntityId, accountLegalEntityId);
        return WithAgreement(employerAgreement, accountLegalEntity);
    }

    public EmployerAgreementBuilder WithSignedAgreement(long accountId, long legalEntityId, long accountLegalEntityId, int agreementVersion, DateTime signeDateTime, out EmployerAgreement employerAgreement)
    {
        WithAgreement(accountId, legalEntityId, accountLegalEntityId, agreementVersion, EmployerAgreementStatus.Signed, out employerAgreement);
        employerAgreement.SignedDate = signeDateTime;
        return this;
    }

    public EmployerAgreementBuilder WithPendingAgreement(long accountId, long legalEntityId, long accountLegalEntityId, int agreementVersion)
    {
        return WithAgreement(accountId, legalEntityId, accountLegalEntityId, agreementVersion, EmployerAgreementStatus.Pending);
    }

    public EmployerAgreementBuilder WithPendingAgreement(long accountId, long legalEntityId, long accountLegalEntityId, int agreementVersion, out EmployerAgreement employerAgreement)
    {
        return WithAgreement(accountId, legalEntityId, accountLegalEntityId, agreementVersion, EmployerAgreementStatus.Pending, out employerAgreement);
    }

    public EmployerAgreementBuilder WithAgreement(long accountId, long legalEntityId, long accountLegalEntityId, int agreementVersion, EmployerAgreementStatus status, out EmployerAgreement employerAgreement)
    {
        var template = EnsureTemplate(agreementVersion);
        var accountLegalEntity = EnsureAccountLegalEntity(accountId, legalEntityId, accountLegalEntityId);

        employerAgreement = new EmployerAgreement
        {
            Id = EmployerAgreements.Count + 1000, // offset from account Ids so that hashing mock won't get clashing ids
            Template = template,
            TemplateId = template.Id,
            StatusId = status,
            AccountLegalEntityId = accountLegalEntityId
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

        var agreementHash = $"AGR{employerAgreement.Id:D3}";

        AddHash(employerAgreement.Id, agreementHash);
        return this;
    }

    public EmployerAgreementBuilder WithAgreement(long accountId, long legalEntityId, long accountLegalEntityId, int agreementVersion, EmployerAgreementStatus status)
    {
        return WithAgreement(accountId, legalEntityId, accountLegalEntityId, agreementVersion, status, out _);
    }

    public EmployerAgreementBuilder EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities()
    {
        EmployerAgreement FindVersionToUse(AccountLegalEntity ale, EmployerAgreementStatus status)
        {
            return ale.Agreements.Where(a => a.StatusId == status)
                                 .MaxBy(a => a.Template.VersionNumber);
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
        var accountsDbSet = Accounts.AsQueryable().BuildMockDbSet();
        var agreementsDbSet = EmployerAgreements.AsQueryable().BuildMockDbSet();
        var accountLegalEntityDbSet = AccountLegalEntities.AsQueryable().BuildMockDbSet();
        var agreementTemplateEntityDbSet = AgreementTemplates.AsQueryable().BuildMockDbSet();
        var userEntityDbSet = Users.AsQueryable().BuildMockDbSet();
        var membershipEntityDbSet = Memberships.AsQueryable().BuildMockDbSet();

        EmployerAccountDbContextMock
            .Setup(db => db.Accounts)
            .Returns(accountsDbSet.Object);

        EmployerAccountDbContextMock
            .Setup(db => db.Agreements)
            .Returns(agreementsDbSet.Object);

        EmployerAccountDbContextMock
            .Setup(db => db.AccountLegalEntities)
            .Returns(accountLegalEntityDbSet.Object);

        EmployerAccountDbContextMock
            .Setup(db => db.AgreementTemplates)
            .Returns(agreementTemplateEntityDbSet.Object);

        EmployerAccountDbContextMock
            .Setup(db => db.Users)
            .Returns(userEntityDbSet.Object);

        EmployerAccountDbContextMock
            .Setup(db => db.Memberships)
            .Returns(membershipEntityDbSet.Object);

        return this;
    }

    public EmployerAgreementBuilder WithUser(long accountId, string firstName, string lastName, Role role, out User user)
    {
        var account = GetAccount(accountId);

        user = new User
        {
            Ref = Guid.NewGuid(),
            Id = Users.Count + 1,
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
            Account = account,
            Role = role
        };

        Memberships.Add(membership);

        return this;
    }

    private AgreementTemplate EnsureTemplate(int agreementVersion)
    {
        var template = AgreementTemplates.FirstOrDefault(ag => ag.VersionNumber == agreementVersion);
        if (template == null)
        {
            AgreementTemplates.Add(template = new AgreementTemplate { VersionNumber = agreementVersion, Id = agreementVersion });
        }

        return template;
    }

    private LegalEntity EnsureLegalEntity(long legalEntityId)
    {
        var legalEntity = LegalEntities.FirstOrDefault(le => le.Id == legalEntityId);

        if (legalEntity == null)
        {
            LegalEntities.Add(legalEntity = new LegalEntity { Id = legalEntityId });
        }

        return legalEntity;
    }

    public Account GetAccount(long accountId)
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
        EncodingServiceMock.Setup(c => c.Decode(hashValue, It.IsAny<EncodingType>())).Returns(id);
        EncodingServiceMock.Setup(c => c.Encode(id, It.IsAny<EncodingType>())).Returns(hashValue);
    }

    private AccountLegalEntity EnsureAccountLegalEntity(long accountId, long legalEntityId, long accountLegalEntityId)
    {
        var accountLegalEntity = AccountLegalEntities.FirstOrDefault(ale => ale.AccountId == accountId && ale.LegalEntityId == legalEntityId);

        if (accountLegalEntity != null)
        {
            return accountLegalEntity;
        }

        var legalEntity = EnsureLegalEntity(legalEntityId);
        var account = GetAccount(accountId);

        AccountLegalEntities.Add(accountLegalEntity = new AccountLegalEntity
        {
            AccountId = accountId,
            LegalEntityId = legalEntityId,
            Account = account,
            LegalEntity = legalEntity,
            Id = accountLegalEntityId
        });

        account.AccountLegalEntities.Add(accountLegalEntity);
        legalEntity.AccountLegalEntities.Add(accountLegalEntity);

        return accountLegalEntity;
    }
}