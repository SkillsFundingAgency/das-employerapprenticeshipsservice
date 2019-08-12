using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetFinancialStatistics;
using SFA.DAS.EAS.Domain.Models.Payments;
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
                .And.Match<GetFinancialStatisticsResponse>(r2 =>
                    r2.Statistics != null &&
                    r2.Statistics.TotalPayments == 2));
        }
    }

    public class GetStatisticsQueryTestsFixtures : FluentTestFixture
    {
        public Mock<EmployerFinanceDbContext> FinancialDb { get; }
        public GetFinancialStatisticsQueryHandler Handler { get; }
        public List<Payment> Payments { get; set; }
        public GetFinancialStatisticsQuery Query { get; }

        public GetStatisticsQueryTestsFixtures()
        {
            Payments = new List<Payment>
            {
                new Payment(), 
                new Payment()
            };

            FinancialDb = new Mock<EmployerFinanceDbContext>();

            FinancialDb.Setup(d => d.Payments).Returns(new DbSetStub<Payment>(Payments));

            Handler = new GetFinancialStatisticsQueryHandler(FinancialDb.Object);
            Query = new GetFinancialStatisticsQuery();

            QueryFutureManager.AllowQueryBatch = false;
        }

        public async Task<GetFinancialStatisticsResponse> Handle()
        {
            return await Handler.Handle(Query);
        }
    }
}