using System;
using System.Collections.Generic;
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
using Z.EntityFramework.Plus;
using FluentTestFixture = SFA.DAS.Testing.FluentTestFixture;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetStatisticsTests
{
    [TestFixture]
    public class GetStatisticsQueryTests : Testing.FluentTest<GetStatisticsQueryTestsFixtures>
    {
        [Test]
        public Task Handle_WhenIGetStatistics_ThenShouldReturnResponse()
        {
            return RunAsync(f => f.Handle(), (f, r) => r.Should().NotBeNull()
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
                new Account(),
                new Account()
            };

            LegalEntities = new List<LegalEntity>
            {
                new LegalEntity(),
                new LegalEntity(),
                new LegalEntity(),
                new LegalEntity(),
            };

            PayeSchemes = new List<Paye>
            {
                new Paye(),
                new Paye(),
                new Paye(),
                new Paye()
            };

            Agreements = new List<EmployerAgreement>
            {
                new EmployerAgreement(),
                new EmployerAgreement { StatusId = EmployerAgreementStatus.Signed },
                new EmployerAgreement { StatusId = EmployerAgreementStatus.Signed },
                new EmployerAgreement { StatusId = EmployerAgreementStatus.Signed },
                new EmployerAgreement { StatusId = EmployerAgreementStatus.Signed },
                new EmployerAgreement { StatusId = EmployerAgreementStatus.Signed }
            };

            AccountsDb = new Mock<EmployerAccountsDbContext>();

            AccountsDb.Setup(d => d.Accounts).Returns(new DbSetStub<EmployerAccounts.Models.Account.Account>(Accounts));
            AccountsDb.Setup(d => d.LegalEntities).Returns(new DbSetStub<LegalEntity>(LegalEntities));
            AccountsDb.Setup(d => d.Payees).Returns(new DbSetStub<Paye>(PayeSchemes));
            AccountsDb.Setup(d => d.Agreements).Returns(new DbSetStub<EmployerAgreement>(Agreements));

            Handler = new GetStatisticsQueryHandler(new Lazy<EmployerAccountsDbContext>(() => AccountsDb.Object));
            Query = new GetStatisticsQuery();

            QueryFutureManager.AllowQueryBatch = false;
        }

        public async Task<GetStatisticsResponse> Handle()
        {
            return await Handler.Handle(Query, CancellationToken.None);
        }
    }
}