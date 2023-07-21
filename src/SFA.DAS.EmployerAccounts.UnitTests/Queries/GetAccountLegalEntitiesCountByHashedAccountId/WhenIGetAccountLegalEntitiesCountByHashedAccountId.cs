using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountLegalEntitiesCountByHashedAccountId;

[TestFixture]
public class GetAccountLegalEntitiesCountByHashedAccountIdTests : Testing.FluentTest<GetAccountLegalEntitiesCountByHashedAccountIdTestsFixture>
{
    [Test]
    public Task Handle_WhenHandlingAGetAccountLegalEntitiesCountByHashedAccountIdRequest_ThenShouldReturnTheNumberOfLegalEntitiesForAnAccount()
    {
        return TestAsync(f => f.Handle(), (f, r) =>
        {
            r.Should().NotBeNull();
            r.LegalEntitiesCount.Should().Be(1);
        });
    }
}

public class GetAccountLegalEntitiesCountByHashedAccountIdTestsFixture
{
    private const string HashedId = "123";
    private const long AccountId = 456;

    public GetAccountLegalEntitiesCountByHashedAccountIdRequest GetAccountLegalEntitiesCountByHashedAccountIdRequest { get; set; }
    public IRequestHandler<GetAccountLegalEntitiesCountByHashedAccountIdRequest, GetAccountLegalEntitiesCountByHashedAccountIdResponse> Handler { get; set; }
    public Mock<EmployerAccountsDbContext> Db { get; set; }
    public List<AccountLegalEntity> AccountLegalEntities { get; set; }
    public Mock<IEncodingService> EncodingService { get; set; }
    public Mock<IValidator<GetAccountLegalEntitiesCountByHashedAccountIdRequest>> RequestValidator { get; set; }

    public GetAccountLegalEntitiesCountByHashedAccountIdTestsFixture()
    {
        long decodeAccountId = AccountId;

        GetAccountLegalEntitiesCountByHashedAccountIdRequest = new GetAccountLegalEntitiesCountByHashedAccountIdRequest { HashedAccountId = HashedId };
        Db = new Mock<EmployerAccountsDbContext>();

        RequestValidator = new Mock<IValidator<GetAccountLegalEntitiesCountByHashedAccountIdRequest>>();
        RequestValidator.Setup(x => x.Validate(It.IsAny<GetAccountLegalEntitiesCountByHashedAccountIdRequest>())).Returns(new ValidationResult());

        EncodingService = new Mock<IEncodingService>();
        EncodingService.Setup(m => m.TryDecode(HashedId, EncodingType.AccountId, out decodeAccountId)).Returns(true);

        AccountLegalEntities = new List<AccountLegalEntity>
        {
            new AccountLegalEntity { AccountId = AccountId },
            new AccountLegalEntity { AccountId = 0 }
        };

        var mockDbSet = AccountLegalEntities.AsQueryable().BuildMockDbSet();

        Db.Setup(d => d.AccountLegalEntities).Returns(mockDbSet.Object);

        Handler = new GetAccountLegalEntitiesCountByHashedAccountIdQueryHandler(
            EncodingService.Object,
            new Lazy<EmployerAccountsDbContext>(() => Db.Object), 
            RequestValidator.Object);
    }

    public Task<GetAccountLegalEntitiesCountByHashedAccountIdResponse> Handle()
    {
        return Handler.Handle(GetAccountLegalEntitiesCountByHashedAccountIdRequest, CancellationToken.None);
    }
}