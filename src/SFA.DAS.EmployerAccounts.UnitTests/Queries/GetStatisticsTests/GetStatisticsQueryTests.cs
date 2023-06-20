using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetStatistics;
using SFA.DAS.EmployerAccounts.TestCommon;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetStatisticsTests;

[TestFixture]
public class GetStatisticsQueryTests : Testing.FluentTest<GetStatisticsQueryTestsFixtures>
{
    [Test]
    public Task Handle_WhenIGetStatistics_ThenShouldReturnResponse()
    {
        return TestAsync(f => f.Handle(), (f, r) => r.Should().NotBeNull()
            .And.Match<GetStatisticsResponse>(r2 =>
                r2.Statistics != null &&
                r2.Statistics.TotalAccounts == 2 &&
                r2.Statistics.TotalLegalEntities == 4 &&
                r2.Statistics.TotalPayeSchemes == 4 &&
                r2.Statistics.TotalAgreements == 5));
    }
}

public class GetStatisticsQueryTestsFixtures : FluentTestFixture
{
    public List<Account> Accounts { get; }
    public Mock<EmployerAccountsDbContext> AccountsDb { get; }
    public List<EmployerAgreement> Agreements { get; set; }
    public GetStatisticsQueryHandler Handler { get; }
    public List<LegalEntity> LegalEntities { get; }
    public List<Paye> PayeSchemes { get; }
    public GetStatisticsQuery Query { get; }

    public GetStatisticsQueryTestsFixtures()
    {
        Accounts = new List<Account>
        {
            new(),
            new()
        };

        LegalEntities = new List<LegalEntity>
        {
            new(),
            new(),
            new(),
            new(),
        };

        PayeSchemes = new List<Paye>
        {
            new(),
            new(),
            new(),
            new()
        };

        Agreements = new List<EmployerAgreement>
        {
            new(),
            new() { StatusId = EmployerAgreementStatus.Signed },
            new() { StatusId = EmployerAgreementStatus.Signed },
            new() { StatusId = EmployerAgreementStatus.Signed },
            new() { StatusId = EmployerAgreementStatus.Signed },
            new() { StatusId = EmployerAgreementStatus.Signed }
        };

        AccountsDb = new Mock<EmployerAccountsDbContext>();

        var mockAccountsDbSet = Accounts.AsQueryable().BuildMockDbSet();
        var mockLegalEntitiesDbSet = LegalEntities.AsQueryable().BuildMockDbSet();
        var mockPayeSchemesDbSet = PayeSchemes.AsQueryable().BuildMockDbSet();
        var mockAgreementsDbSet = Agreements.AsQueryable().BuildMockDbSet();

        AccountsDb.Setup(d => d.Accounts).Returns(mockAccountsDbSet.Object);
        AccountsDb.Setup(d => d.LegalEntities).Returns(mockLegalEntitiesDbSet.Object);
        AccountsDb.Setup(d => d.Payees).Returns(mockPayeSchemesDbSet.Object);
        AccountsDb.Setup(d => d.Agreements).Returns(mockAgreementsDbSet.Object);

        Handler = new GetStatisticsQueryHandler(new Lazy<EmployerAccountsDbContext>(() => AccountsDb.Object));
        Query = new GetStatisticsQuery();
    }

    public Task<GetStatisticsResponse> Handle()
    {
        return Handler.Handle(Query, CancellationToken.None);
    }
}