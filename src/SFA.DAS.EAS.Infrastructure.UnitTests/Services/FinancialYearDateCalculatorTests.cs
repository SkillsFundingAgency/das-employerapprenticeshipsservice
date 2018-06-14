using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Models;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services
{
    [TestFixture]
    public class FinancialYearDateCalculatorTests
    {
        [Test]
        public void Constructor_ValidCall_ShouldNotThrowException()
        {
            var dc = new FinancialYearDateCalculator(new FinancialYearCutoff());
        }

        [TestCase("2018-01-01 11:12:13.123", 2018)]
        [TestCase("2018-04-19 23:59:59.999", 2018)]
        [TestCase("2018-04-20 00:00:00.000", 2019)]
        [TestCase("2018-04-20 00:00:00.001", 2019)]
        [TestCase("2019-12-31 23:59:59.999", 2020)]
        [TestCase("2020-01-01 00:00:00.000", 2020)]
        public void GetEndFinancialYear_VariousDates_ShouldCalculateCorrectYear(string pointInTime, int expectedEndYear)
        {
            Assert.IsTrue(DateTime.TryParseExact(pointInTime, "yyyy-MM-dd HH:mm:ss.FFF", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTime dateTime), $"The string '{pointInTime}' could not be parsed");

            var dc = new FinancialYearDateCalculator(new FinancialYearCutoff());
            var actualEndYear = dc.GetEndFinancialYear(dateTime);

            Assert.AreEqual(expectedEndYear, actualEndYear);
        }

        [TestCase(2018)]
        [TestCase(2019)]
        [TestCase(2020)]
        public void GetYearEnd_VariousYears_ShouldCalculateYearEnd(int endYear)
        {
            var expectedYearEnd = new DateTime(endYear, 4, 19, 23, 59, 59, 999);
            var dc = new FinancialYearDateCalculator(new FinancialYearCutoff());
            var actualYearEnd = dc.GetYearEnd(endYear);

            Assert.AreEqual(actualYearEnd, expectedYearEnd);
        }

        [TestCase(2018)]
        [TestCase(2019)]
        [TestCase(2020)]
        public void GetYearStart_VariousYears_ShouldCalculateYearStart(int endYear)
        {
            var expectedYearStart = new DateTime(endYear-1, 4, 20, 0, 0, 0, 0);
            var dc = new FinancialYearDateCalculator(new FinancialYearCutoff());
            var actualYearStart = dc.GetYearStart(endYear);

            Assert.AreEqual(actualYearStart, expectedYearStart);
        }

    }
}
