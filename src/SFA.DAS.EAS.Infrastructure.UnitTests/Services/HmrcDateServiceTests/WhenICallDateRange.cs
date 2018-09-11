using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.HmrcDateServiceTests
{
    [TestFixture]
    public class WhenICallIsDateInPayrollPeriod
    {
        [TestCase("17-18", 1, "2017-03-31 23:59:59.999", false)]
        [TestCase("17-18", 1, "2017-04-19 23:59:59.999", false)]
        [TestCase("17-18", 1, "2017-04-20 00:00:00.000", true)]
        [TestCase("17-18", 1, "2017-04-20 00:00:00.001", true)]
        [TestCase("17-18", 1, "2017-05-01 00:00:00.000", true)]
        [TestCase("17-18", 9, "2018-01-01 00:00:00.000", true)]
        [TestCase("17-18", 12, "2018-04-19 23:59:59.999", true)]
        [TestCase("17-18", 12, "2018-04-20 00:00:00.000", false)]
        public void ThenShouldCorrectlyIndicateAnswer(string payrollYear, int payrollMonth, string testDateParam, bool expectedResult)
        {
            var hmrcDateService = new HmrcDateService();

            var testDate = DateTime.ParseExact(testDateParam, "yyyy-MM-dd HH:mm:ss.fff", new DateTimeFormatInfo(), DateTimeStyles.None);

            var actualResult = hmrcDateService.IsDateInPayrollPeriod(payrollYear, payrollMonth, testDate);

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
