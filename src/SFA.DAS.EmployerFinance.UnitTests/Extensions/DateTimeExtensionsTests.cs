using NUnit.Framework;
using SFA.DAS.EmployerFinance.Extensions;
using System;

namespace SFA.DAS.EmployerFinance.UnitTests.Extensions
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [TestCase(2022, 2, 1, "2021/22")]
        [TestCase(2022, 4, 1, "2021/22")]
        [TestCase(2022, 4, 20, "2022/23")]
        [TestCase(2022, 5, 1, "2022/23")]
        public void ToFinancialYearStringReturnsCorrectString(int year, int month, int day, string expected)
        {
            var dateTime = new DateTime(year, month, day);
            var result = dateTime.ToFinancialYearString();

            Assert.AreEqual(expected, result);
        }

        [TestCase(2022, 2, 1, "2022/23")]
        [TestCase(2022, 4, 1, "2022/23")]
        [TestCase(2022, 4, 20, "2023/24")]
        [TestCase(2022, 5, 1, "2023/24")]
        public void ToNextFinancialYearStringReturnsCorrectString(int year, int month, int day, string expected)
        {
            var dateTime = new DateTime(year, month, day);
            var result = dateTime.ToNextFinancialYearString();

            Assert.AreEqual(expected, result);
        }

        [TestCase(2022, 2, 1, "2023/24")]
        [TestCase(2022, 4, 1, "2023/24")]
        [TestCase(2022, 4, 20, "2024/25")]
        [TestCase(2022, 5, 1, "2024/25")]
        public void ToYearAfterNextFinancialYearStringReturnsCorrectString(int year, int month, int day, string expected)
        {
            var dateTime = new DateTime(year, month, day);
            var result = dateTime.ToYearAfterNextFinancialYearString();

            Assert.AreEqual(expected, result);
        }
    }
}
