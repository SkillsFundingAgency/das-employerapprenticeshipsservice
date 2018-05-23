using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetStatistics;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetStatisticsTests
{
    [TestFixture]
    public class GetStatisticsQueryTests : FluentTest<GetStatisticsQueryTestsFixtures>
    {
        [Test]
        public void Handle_WhenIGetStatistics_ThenShouldReturnResponse()
        {
            RunAsync(f => f.Handle(), f => f.Response.Should().NotBeNull());
        }
    }

    public class GetStatisticsQueryTestsFixtures : FluentTestFixture
    {
        public Mock<EmployerAccountDbContext> AccountsDb { get; set; }
        public Mock<EmployerFinancialDbContext> FinancialDb { get; set; }

        public GetStatisticsQueryHandler Handler { get; set; }
        public GetStatisticsQuery Query { get; set; }
        public GetStatisticsResponse Response { get; set; }

        public GetStatisticsQueryTestsFixtures()
        {
            AccountsDb = new Mock<EmployerAccountDbContext>();
            FinancialDb = new Mock<EmployerFinancialDbContext>();

            AccountsDb.Setup(d => d.Accounts).Returns(new DbSetStub<Domain.Models.Account.Account>(new List<Domain.Models.Account.Account>()));
            AccountsDb.Setup(d => d.LegalEntities).Returns(new DbSetStub<LegalEntity>(new List<LegalEntity>()));
            AccountsDb.Setup(d => d.Payees).Returns(new DbSetStub<Paye>(new List<Paye>()));
            AccountsDb.Setup(d => d.Agreements).Returns(new DbSetStub<EmployerAgreement>());
            FinancialDb.Setup(d => d.Payments).Returns(new DbSetStub<Payment>());

            Handler = new GetStatisticsQueryHandler(FinancialDb.Object, AccountsDb.Object);
            Query = new GetStatisticsQuery();
        }

        public async Task Handle()
        {
            Response = await Handler.Handle(Query);
        }
    }
}
