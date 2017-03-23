using System;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.HmrcDateServiceTests
{
    public class WhenICompareTheSubmissionDateToPayrolDate
    {
        private HmrcDateService _hmrcDateService;

        [SetUp]
        public void Arrange()
        {
            _hmrcDateService = new HmrcDateService();
        }

        [Test]
        public void ThenIfTheSubmissionDateIsOutsideOfThePayrolDateThenFalseIsReturned()
        {
            //Arrange
            var payroll = "16-17";
            var submissionDate = new DateTime(2017, 05, 01);

            //Act
            var actual = _hmrcDateService.IsSubmissionDateInPayrollYear(payroll, submissionDate);


            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void ThenIfTheSubmissionDateIsInsideOfThePayrolDateThenTrueIsReturned()
        {
            //Arrange
            var payroll = "16-17";
            var submissionDate = new DateTime(2017, 04, 30);

            //Act
            var actual = _hmrcDateService.IsSubmissionDateInPayrollYear(payroll, submissionDate);

            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void ThenIftheSubmissionDateIsValidForAnAdjustmentAndMonthIsNotTwelveThenFalseIsReturned()
        {
            //Arrange
            var payroll = "16-17";
            var expectedDate = new DateTime(2018,04,30);

            //Act
            var actual = _hmrcDateService.IsSubmissionEndOfYearAdjustment(payroll, 11, expectedDate);

            //Assert
            Assert.IsFalse(actual);
        }

        [TestCase("2016-04-30", false)]
        [TestCase("2017-04-30", false)]
        [TestCase("2017-05-01", true)]
        [TestCase("2018-04-30", true)]
        public void ThenIfTheSubmissionDateIsGreaterThanThePayrollYearThenTrueIsReturned(string submissionDate, bool expectedResult)
        {
            //Arrange
            var payroll = "16-17";
            var expectedDate = DateTime.Parse(submissionDate);

            //Act
            var actual = _hmrcDateService.IsSubmissionEndOfYearAdjustment(payroll, 12, expectedDate);

            //Assert
            Assert.AreEqual(expectedResult, actual);
        }


    }
}
