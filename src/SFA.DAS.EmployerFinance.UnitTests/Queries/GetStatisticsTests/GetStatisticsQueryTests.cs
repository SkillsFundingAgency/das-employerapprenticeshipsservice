﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.EmployerAgreement;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Queries.GetStatistics;
using SFA.DAS.Testing;
using SFA.DAS.Testing.EntityFramework;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetStatisticsTests
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
                    r2.Statistics.TotalPayments == 2));
        }
    }

    public class GetStatisticsQueryTestsFixtures : FluentTestFixture
    {
        public List<Account> Accounts { get; }
        public Mock<EmployerAccountsDbContext> AccountsDb { get; }
        public List<EmployerAgreement> Agreements { get; set; }
        public Mock<EmployerFinanceDbContextReadOnly> FinancialDb { get; }
        public GetStatisticsQueryHandler Handler { get; }
        public List<LegalEntity> LegalEntities { get; }
        public List<Paye> PayeSchemes { get; }
        public List<Payment> Payments { get; set; }
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

            Payments = new List<Payment>
            {
                new Payment(), 
                new Payment()
            };

            AccountsDb = new Mock<EmployerAccountsDbContext>();
            FinancialDb = new Mock<EmployerFinanceDbContextReadOnly>();

            AccountsDb.Setup(d => d.Accounts).Returns(new DbSetStub<Account>(Accounts));
            AccountsDb.Setup(d => d.LegalEntities).Returns(new DbSetStub<LegalEntity>(LegalEntities));
            AccountsDb.Setup(d => d.Payees).Returns(new DbSetStub<Paye>(PayeSchemes));
            AccountsDb.Setup(d => d.Agreements).Returns(new DbSetStub<EmployerAgreement>(Agreements));
            FinancialDb.Setup(d => d.Payments).Returns(new DbSetStub<Payment>(Payments));

            Handler = new GetStatisticsQueryHandler(FinancialDb.Object);
            Query = new GetStatisticsQuery();

            QueryFutureManager.AllowQueryBatch = false;
        }

        public async Task<GetStatisticsResponse> Handle()
        {
            return await Handler.Handle(Query);
        }
    }
}