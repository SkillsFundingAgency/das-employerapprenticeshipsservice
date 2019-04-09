using System.Data;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable]
    public class ExpiredFundsExtensionsTests : FluentTest<ExpiredFundsExtensionsTestsFixture>
    {
        [Test]
        public void ToExpiredFundsDataTable_WhenCreatingTable_ShouldReturnPopulatedTable()
        {
            Run(f => f.ToExpiredFundsDataTable(), (f, r) => r.Should().Match<DataTable>(
                dt => dt.Rows.Count == 1
                && dt.Rows[0].Field<int>("CalendarPeriodYear") == 2020
                && dt.Rows[0].Field<int>("CalendarPeriodMonth") == 2
                && dt.Rows[0].Field<decimal>("Amount") == 2.22m)
            );
        }
    }

    public class ExpiredFundsExtensionsTestsFixture
    {
        public DataTable ToExpiredFundsDataTable()
        {
            return new[] {new ExpiredFund { CalendarPeriodYear = 2020, CalendarPeriodMonth = 2, Amount = 2.22m} }.ToExpiredFundsDataTable();
        }
    }
}
