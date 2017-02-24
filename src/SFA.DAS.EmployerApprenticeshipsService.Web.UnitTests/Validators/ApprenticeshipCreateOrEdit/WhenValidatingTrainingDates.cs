using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingTrainingDates : ApprenticeshipValidationTestBase
    {
        [Test]
        public void ShouldIfStartDateBeforeMay2017()
        {
            ValidModel.StartDate = new DateTimeViewModel(30, 4, 2017);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The start date must not be earlier than 1 May 2017");
        }

        [TestCase(31, 2, 2121, "The Learning start date is not valid")]
        [TestCase(5, null, 2121, "The Learning start date is not valid")]
        [TestCase(5, 9, null, "The Learning start date is not valid")]
        [TestCase(5, 9, -1, "The Learning start date is not valid")]
        [TestCase(0, 0, 0, "The Learning start date is not valid")]
        [TestCase(1, 18, 2121, "The Learning start date is not valid")]
        //[TestCase(5, 9, 1998, "Learner start date must be in the future")]
        public void ShouldFailValidationForStartDate(int? day, int? month, int? year, string expected)
        {

            ValidModel.StartDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(null, null, null)]
        [TestCase(5, 9, 2100)]
        [TestCase(1, 1, 2023)]
        [TestCase(null, 9, 2067)]
        public void ShouldNotFailValidationForStartDate(int? day, int? month, int? year)
        {
            ValidModel.StartDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(31, 2, 2121, "The Learning planned end date is not valid")]
        [TestCase(5, null, 2121, "The Learning planned end date is not valid")]
        [TestCase(5, 9, null, "The Learning planned end date is not valid")]
        [TestCase(5, 9, -1, "The Learning planned end date is not valid")]
        [TestCase(0, 0, 0, "The Learning planned end date is not valid")]
        [TestCase(1, 18, 2121, "The Learning planned end date is not valid")]
        [TestCase(5, 9, 1998, "The Learning planned end date must not be in the past")]
        public void ShouldFailValidationForPlanedEndDate(int? day, int? month, int? year, string expected)
        {

            ValidModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(null, null, null)]
        [TestCase(5, 9, 2100)]
        [TestCase(1, 1, 2023)]
        [TestCase(null, 9, 2067)]
        public void ShouldNotFailValidationForPlannedEndDate(int? day, int? month, int? year)
        {
            ValidModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldFailValidationForPlanedEndDate()
        {
            var date = DateTime.Now;
            ValidModel.EndDate = new DateTimeViewModel(date.Day, date.Month, date.Year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The Learning planned end date must not be in the past");
        }

        [Test]

        public void ShouldFailIfStartDateIsAfterEndDate()
        {
            ValidModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            ValidModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The Learning planned end date must not be on or before the Learning start date");
        }

        [Test]
        public void ShouldNotFailIfStartDateIsNull()
        {
            ValidModel.StartDate = null;
            ValidModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldNotFailIfEndDateIsNull()
        {
            ValidModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            ValidModel.EndDate = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

    }
}
