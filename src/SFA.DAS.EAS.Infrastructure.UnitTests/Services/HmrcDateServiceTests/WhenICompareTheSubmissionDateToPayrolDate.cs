﻿using System;
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
            var submissionDate = new DateTime(2017, 04, 01);

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
            var submissionDate = new DateTime(2017, 03, 31);

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

        [TestCase("2016-01-30", false)]
        [TestCase("2017-03-31", false)]
        [TestCase("2017-04-01", false)]
        [TestCase("2017-04-22", true)]
        [TestCase("2018-05-30", true)]
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

        [Test]
        public void ThenIfThePayrollPeriodIsInTheFutureThenTrueIsReturned()
        {
            //Arrange
            var payroll = $"{DateTime.Now.AddYears(1).ToString("yy")}-{DateTime.Now.AddYears(2).ToString("yy")}";
            var dateProcessed = DateTime.Now;

            
            for (var i = 1; i <= 12; i++)
            {
                //Act
                var actual = _hmrcDateService.IsSubmissionForFuturePeriod(payroll, i, dateProcessed);

                //Assert
                Assert.IsTrue(actual);
            }
            
        }

        [Test]
        public void ThenPayrollPeriodsAreProcessedInArrears()
        {
            //Arrange
            var dateProcessed = new DateTime(2017, 04, 18);

            //Act
            var actual = _hmrcDateService.IsSubmissionForFuturePeriod("16-17", 12, dateProcessed);

            //Arrange
            Assert.IsTrue(actual);
        }


        [Test]
        public void ThenPayrollPeriodsAreProcessedInArrearsAndAfterTheTwentyFirstAreProcessed()
        {
            //Arrange
            var dateProcessed = new DateTime(2017, 04, 20);

            //Act
            var actual = _hmrcDateService.IsSubmissionForFuturePeriod("16-17", 12, dateProcessed);

            //Arrange
            Assert.IsFalse(actual);
        }

        [Test]
        public void ThenIfThePayrollPeriodIsForTheCorrectSubmissionDateThenTrueIsReturned()
        {
            //Arrange
            var dateProcessed = new DateTime(2017,03,16);

            //Act
            var actual = _hmrcDateService.IsSubmissionForFuturePeriod("16-17", 12, dateProcessed);

            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void ThenIfThePayrollPeriodIsInThePastThenFalseIsReturned()
        {
            //Arrange
            var dateProcessed = new DateTime(2017, 03, 16);

            //Act
            var actual = _hmrcDateService.IsSubmissionForFuturePeriod("15-16", 12, dateProcessed);

            //Assert
            Assert.IsFalse(actual);
        }

        [TestCase("16-17",true)]
        [TestCase("15-16", true)]
        [TestCase("17-18", false)]
        [TestCase("", false)]
        public void ThenIfThePayrollYearIsBeforeTheLevyWasIntroducedFalseIsReturned(string payrollYear, bool expectedResult)
        {
            //Act
            var actual = _hmrcDateService.DoesSubmissionPreDateLevy(payrollYear);

            //Assert
            Assert.AreEqual(expectedResult, actual);
        }
    }
}
