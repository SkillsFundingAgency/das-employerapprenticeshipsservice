using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.AcademicYearTests
{
    [TestFixture]
    public class WhenDeterminingTheAcademicYear
    {
        private Mock<ICurrentDateTime> _currentDateTime;
        private IAcademicYear _academicYear;

        [SetUp]
        public void Arrange()
        {
            _currentDateTime = new Mock<ICurrentDateTime>();
        }

        [TestCase("2017-08-01", "2017-08-01", "2018-07-31")]
        [TestCase("2018-03-01", "2017-08-01", "2018-07-31")]
        [TestCase("2018-07-31", "2017-08-01", "2018-07-31")]
        [TestCase("2018-10-01", "2018-08-01", "2019-07-31")]
        public void ThenAcademicYearRunsAugustToJuly(DateTime currentDate, DateTime expectedYearStart, DateTime expectedYearEnd)
        {
            //Arrange
            _currentDateTime.Setup(x => x.Now).Returns(currentDate);
            _academicYear = new Infrastructure.Services.AcademicYear(_currentDateTime.Object);

            //Act
            var actualStart = _academicYear.CurrentAcademicYearStartDate;
            var actualEnd = _academicYear.CurrentAcademicYearEndDate;

            //Assert
            Assert.AreEqual(expectedYearStart, actualStart);
            Assert.AreEqual(expectedYearEnd, actualEnd);
        }
    }
}
