using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.EAS.LevyAnalyser.Tests
{
    [TestFixture]
    public class PayrollPeriodComparerTests
    {
        [TestCase("18-19", 1, "18-19", 1, true)]
        [TestCase("18-19", 2, "18-19", 2, true)]
        [TestCase("18-19", 3, "18-19", 3, true)]
        [TestCase("18-19", 4, "18-19", 4, true)]
        [TestCase("18-19", 5, "18-19", 5, true)]
        [TestCase("18-19", 6, "18-19", 6, true)]
        [TestCase("18-19", 7, "18-19", 7, true)]
        [TestCase("18-19", 8, "18-19", 8, true)]
        [TestCase("18-19", 9, "18-19", 9, true)]
        [TestCase("18-19", 10, "18-19", 10, true)]
        [TestCase("18-19", 11, "18-19", 11, true)]
        [TestCase("18-19", 12, "18-19", 12, true)]
        [TestCase("18-19", 1, "18-19", 2, false)]
        [TestCase("17-18", 2, "18-19", 2, false)]
        public void Compare_SameYearAndPeriod_ShouldBeEqual(string year1, byte period1, string year2, byte period2, bool expectedEqual)
        {
            var payrollPeriod1 = new PayrollPeriod(year1, period1);
            var payrollPeriod2 = new PayrollPeriod(year2, period2);

            var actualEquals = new PayrollPeriodComparer().Equals(payrollPeriod1, payrollPeriod2);

            Assert.AreEqual(expectedEqual, actualEquals);
        }
    }
}
