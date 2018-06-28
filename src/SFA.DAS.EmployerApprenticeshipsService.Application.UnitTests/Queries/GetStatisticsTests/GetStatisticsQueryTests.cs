using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetStatistics;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetStatisticsTests
{
    [TestFixture]
    public class GetStatisticsQueryTests : FluentTest<GetStatisticsQueryTestsFixtures>
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
                    r2.Statistics.TotalAgreements == 5 &&
                    r2.Statistics.TotalPayments == 2));
        }
    }

    public class GetStatisticsQueryTestsFixtures : FluentTestFixture
    {
        public List<Domain.Models.Account.Account> Accounts { get; }
        public Mock<EmployerAccountDbContext> AccountsDb { get; }
        public List<EmployerAgreement> Agreements { get; set; }
        public Mock<EmployerFinancialDbContext> FinancialDb { get; }
        public GetStatisticsQueryHandler Handler { get; }
        public List<LegalEntity> LegalEntities { get; }
        public List<Paye> PayeSchemes { get; }
        public List<Payment> Payments { get; set; }
        public GetStatisticsQuery Query { get; }

        public GetStatisticsQueryTestsFixtures()
        {
            Accounts = new List<Domain.Models.Account.Account>
            {
                new Domain.Models.Account.Account(),
                new Domain.Models.Account.Account()
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

            Payments = new List<Payment>
            {
                new Payment(), 
                new Payment()
            };

            AccountsDb = new Mock<EmployerAccountDbContext>();
            FinancialDb = new Mock<EmployerFinancialDbContext>();

            AccountsDb.Setup(d => d.Accounts).Returns(new DbSetStub<Domain.Models.Account.Account>(Accounts));
            AccountsDb.Setup(d => d.LegalEntities).Returns(new DbSetStub<LegalEntity>(LegalEntities));
            AccountsDb.Setup(d => d.Payees).Returns(new DbSetStub<Paye>(PayeSchemes));
            AccountsDb.Setup(d => d.Agreements).Returns(new DbSetStub<EmployerAgreement>(Agreements));
            FinancialDb.Setup(d => d.Payments).Returns(new DbSetStub<Payment>(Payments));

            Handler = new GetStatisticsQueryHandler(new Lazy<EmployerAccountDbContext>(() => AccountsDb.Object), FinancialDb.Object);
            Query = new GetStatisticsQuery();

            QueryFutureManager.AllowQueryBatch = false;
        }

        public async Task<GetStatisticsResponse> Handle()
        {
            return await Handler.Handle(Query);
        }
    }
}