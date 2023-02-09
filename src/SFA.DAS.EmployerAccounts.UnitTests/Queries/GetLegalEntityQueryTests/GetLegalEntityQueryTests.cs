using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetLegalEntity;
using SFA.DAS.EmployerAccounts.TestCommon;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;
using SFA.DAS.EmployerAccounts.TestCommon.Extensions;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetLegalEntityQueryTests;

[TestFixture]
public class GetLegalEntityQueryTests : Testing.FluentTest<GetLegalEntityQueryTestsFixture>
{

    [Test]
    public Task Handle_WhenGettingLegalEntity_WithAllAgreements_ThenShouldReturnLegalEntity()
    {
        return TestAsync(f => f.Handle(), (f, r) => r.Should().NotBeNull()
            .And.Match<GetLegalEntityResponse>(r2 =>
                r2.LegalEntity.LegalEntityId == f.LegalEntity.Id &&
                r2.LegalEntity.Agreements.Count == 5 &&
                r2.LegalEntity.Agreements.Any(a => a.Template.VersionNumber == 1 && a.StatusId == EmployerAgreementStatus.Removed) &&
                r2.LegalEntity.Agreements.Any(a => a.Template.VersionNumber == 1 && a.StatusId == EmployerAgreementStatus.Signed) &&
                r2.LegalEntity.Agreements.Any(a => a.Template.VersionNumber == 2 && a.StatusId == EmployerAgreementStatus.Signed) &&
                r2.LegalEntity.Agreements.Any(a => a.Template.VersionNumber == 3 && a.StatusId == EmployerAgreementStatus.Pending) &&
                r2.LegalEntity.Agreements.Any(a => a.Template.VersionNumber == 3 && a.StatusId == EmployerAgreementStatus.Expired) &&
                r2.LegalEntity.Agreements.Any(a => a.Template.AgreementType == f.LegalEntity.AccountLegalEntities.First().Agreements.First().Template.AgreementType)
            ));
    }

    [Test]
    public async Task Handle_WhenGettingLegalEntity_ThenShouldReturnTheCorrectEmailOfUserWhoSignedTheAgreement()
    {
        var f = new GetLegalEntityQueryTestsFixture();
        var result = await f.Handle();

        result.LegalEntity.Agreements.First(a => a.Template.VersionNumber == 1 && a.StatusId == EmployerAgreementStatus.Signed).SignedByEmail.Should().Be(f.UserB.Email);
        result.LegalEntity.Agreements.First(a => a.Template.VersionNumber == 2 && a.StatusId == EmployerAgreementStatus.Signed).SignedByEmail.Should().Be(f.UserA.Email);
    }


    [Test]
    public async Task Handle_WhenGettingLegalEntity_ThenShouldMapRequiredFields()
    {
        var f = new GetLegalEntityQueryTestsFixture();
        var result = await f.Handle();

        var actual = result.LegalEntity;

        actual.Address.Should().NotBeNullOrEmpty();
        actual.Address.Should().Be(f.LegalEntity.AccountLegalEntities.First().Address);
        actual.Name.Should().NotBeNullOrEmpty();
        actual.Name.Should().Be(f.LegalEntity.AccountLegalEntities.First().Name);
        actual.Should().IsEquivalentTo(f.LegalEntity);
    }
}

public class GetLegalEntityQueryTestsFixture : FluentTestFixture
{
    public GetLegalEntityQueryHandler Handler { get; set; }
    public Mock<EmployerAccountsDbContext> Db { get; set; }
    public Account Account { get; private set; }
    public LegalEntity LegalEntity { get; set; }
    public AccountLegalEntity AccountLegalEntity { get; set; }
    public List<LegalEntity> LegalEntities { get; set; }
    public List<AccountLegalEntity> AccountLegalEntities { get; set; }
    public User UserA { get; set; }
    public User UserB { get; set; }
    public List<User> Users { get; set; }
    

    public GetLegalEntityQueryTestsFixture()
    {
        Db = new Mock<EmployerAccountsDbContext>();

        LegalEntities = new List<LegalEntity>();
        AccountLegalEntities = new List<AccountLegalEntity>();
        
        UserA = new User { Id = 1, Email = "UserA@gmail.com" };
        UserB = new User { Id = 2, Email = "UserB@gmail.com" };
        Users = new List<User> { UserA, UserB };
        
        var legalEntitiesDbSet = LegalEntities.AsQueryable().BuildMockDbSet();
        var accountLegalEntitiesDbSet = AccountLegalEntities.AsQueryable().BuildMockDbSet();
        var mockUsersDbSet = Users.AsQueryable().BuildMockDbSet();

        Db.Setup(d => d.LegalEntities).Returns(legalEntitiesDbSet.Object);
        Db.Setup(d => d.AccountLegalEntities).Returns(accountLegalEntitiesDbSet.Object);
        Db.Setup(d => d.Users).Returns(mockUsersDbSet.Object);

        Handler = new GetLegalEntityQueryHandler(new Lazy<EmployerAccountsDbContext>(() => Db.Object));

        SetAccount()
            .SetLegalEntity()
            .SetLegalAccountLegalEntity()
            .AddLegalEntityAgreement(1, EmployerAgreementStatus.Removed, UserA.Id)
            .AddLegalEntityAgreement(1, EmployerAgreementStatus.Signed, UserB.Id)
            .AddLegalEntityAgreement(2, EmployerAgreementStatus.Signed, UserA.Id)
            .AddLegalEntityAgreement(3, EmployerAgreementStatus.Pending)
            .AddLegalEntityAgreement(3, EmployerAgreementStatus.Expired)
            .EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities();
    }

    public Task<GetLegalEntityResponse> Handle()
    {
        return Handler.Handle(
            new GetLegalEntityQuery(
                Account.HashedId,
                LegalEntity.Id
            ), CancellationToken.None);
    }

    public GetLegalEntityQueryTestsFixture SetAccount()
    {
        Account = new Account
        {
            Id = 111111,
            HashedId = "ABC123",
            Name = "ABC123 CORP"
        };

        return this;
    }

    private GetLegalEntityQueryTestsFixture SetLegalEntity()
    {
        LegalEntity = new LegalEntity
        {
            Id = 222222,
            Code = "0123456",
            Sector = "Some Sector",
            Status = "Some Status"
        };

        LegalEntities.Add(LegalEntity);
        return this;
    }

    public GetLegalEntityQueryTestsFixture EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities()
    {
        EmployerAgreement FindVersionToUse(AccountLegalEntity ale, EmployerAgreementStatus status)
        {
            return ale.Agreements.Where(a => a.StatusId == status).MaxBy(a => a.Template.VersionNumber);
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

    private GetLegalEntityQueryTestsFixture SetLegalAccountLegalEntity()
    {
        AccountLegalEntity = new AccountLegalEntity
        {
            Id = AccountLegalEntities.Count + 1,
            Account = Account,
            AccountId = Account.Id,
            LegalEntity = LegalEntity,
            LegalEntityId = LegalEntity.Id,
            Address = "123 High Street",
            Name = "AccountLegalEntity Name"
        };

        LegalEntity.AccountLegalEntities.Add(AccountLegalEntity);
        Account.AccountLegalEntities.Add(AccountLegalEntity);

        AccountLegalEntities.Add(AccountLegalEntity);

        return this;
    }

    private GetLegalEntityQueryTestsFixture AddLegalEntityAgreement(int versionNumber, EmployerAgreementStatus status, long? signedUserId = null)
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