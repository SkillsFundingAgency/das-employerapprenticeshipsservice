using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Interfaces;
using SFA.DAS.EAS.Infrastructure.Models;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services
{
    internal class DateServiceTestFixtures
    {
        public DateServiceTestFixtures()
        {
            SystemDateServiceMock = new Mock<ISystemDateService>();
            FinancialYearDateCalculator = new FinancialYearDateCalculator(new FinancialYearCutoff());
        }

        public Mock<ISystemDateService> SystemDateServiceMock { get; }
        public ISystemDateService SystemDateService => SystemDateServiceMock.Object;

        public IFinancialYearDateCalculator FinancialYearDateCalculator { get; }

        public DateServiceTestFixtures SetCurrentSystemDate(DateTime requriedSystemDate)
        {
            SystemDateServiceMock
                .Setup(sdsm => sdsm.Current)
                .Returns(() => requriedSystemDate);

            return this;
        }

        public DateService CreateDateService()
        {
            return new DateService(FinancialYearDateCalculator, SystemDateService);
        }
    }

    [TestFixture]
    public class DateServiceTests
    {
        [Test]
        public void Constructor_ValidCall_ShouldNotThrowException()
        {
            var fixtues = new DateServiceTestFixtures();

            var ds = fixtues.CreateDateService();
        }

        [TestCase("2018-01-01 11:12:13.123", 2018)]
        [TestCase("2018-04-19 23:59:59.999", 2018)]
        [TestCase("2018-04-20 00:00:00.000", 2019)]
        [TestCase("2018-04-20 00:00:00.001", 2019)]
        [TestCase("2019-12-31 23:59:59.999", 2020)]
        [TestCase("2020-01-01 00:00:00.000", 2020)]
        public void GetCurrentFinancialYear_WithVariousSystemDates_ShouldDetermineCorrectFinancialYear(string systemDateString, int expectedEndYear)
        {
            Assert.IsTrue(DateTime.TryParseExact(systemDateString, "yyyy-MM-dd HH:mm:ss.FFF", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTime systemDateTime), $"The string '{systemDateString}' could not be parsed");

            var fixtues = new DateServiceTestFixtures().SetCurrentSystemDate(systemDateTime);

            var ds = fixtues.CreateDateService();

            var actualFinYear = ds.CurrentFinancialYear;

            Assert.AreEqual(expectedEndYear-1, actualFinYear.StartYear, $"{nameof(FinancialYearDetails.StartYear)} is not set correctly");
            Assert.AreEqual(expectedEndYear, actualFinYear.EndYear, $"{nameof(FinancialYearDetails.EndYear)} is not set correctly");
        }

        [TestCase("2018-01-01 11:12:13.123", 2017)]
        [TestCase("2018-04-20 00:00:00.000", 2018)]
        public void GetPreviousFinancialYear_WithVariousSystemDates_ShouldDetermineCorrectFinancialYear(string systemDateString, int expectedPreviousEndYear)
        {
            Assert.IsTrue(DateTime.TryParseExact(systemDateString, "yyyy-MM-dd HH:mm:ss.FFF", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTime systemDateTime), $"The string '{systemDateString}' could not be parsed");

            var fixtues = new DateServiceTestFixtures().SetCurrentSystemDate(systemDateTime);

            var ds = fixtues.CreateDateService();

            var actualFinYear = ds.PreviousFinancialYear;

            Assert.AreEqual(expectedPreviousEndYear - 1, actualFinYear.StartYear, $"{nameof(FinancialYearDetails.StartYear)} is not set correctly");
            Assert.AreEqual(expectedPreviousEndYear, actualFinYear.EndYear, $"{nameof(FinancialYearDetails.EndYear)} is not set correctly");
        }

        [TestCase("2018-01-01 11:12:13.123", 2019)]
        [TestCase("2018-04-20 00:00:00.000", 2020)]
        public void GetNextFinancialYear_WithVariousSystemDates_ShouldDetermineCorrectFinancialYear(string systemDateString, int expectedNextEndYear)
        {
            Assert.IsTrue(DateTime.TryParseExact(systemDateString, "yyyy-MM-dd HH:mm:ss.FFF", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTime systemDateTime), $"The string '{systemDateString}' could not be parsed");

            var fixtues = new DateServiceTestFixtures().SetCurrentSystemDate(systemDateTime);

            var ds = fixtues.CreateDateService();

            var actualFinYear = ds.NextFinancialYear;

            Assert.AreEqual(expectedNextEndYear - 1, actualFinYear.StartYear, $"{nameof(FinancialYearDetails.StartYear)} is not set correctly");
            Assert.AreEqual(expectedNextEndYear, actualFinYear.EndYear, $"{nameof(FinancialYearDetails.EndYear)} is not set correctly");
        }
    }
}
