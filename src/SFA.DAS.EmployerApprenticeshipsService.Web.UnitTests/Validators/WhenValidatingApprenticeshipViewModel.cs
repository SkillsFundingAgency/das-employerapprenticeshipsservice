using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Models.Types;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.UnitTests.Validators
{
    [TestFixture]
    public sealed class WhenValidatingApprenticeshipViewModel
    {
        private readonly ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator();
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel { FirstName = "TestFirstName", LastName = "TestLastName" };
        }

        [Test]
        public void ULNMustBeNumericAnd10DigitsInLength()
        {
            _validModel.ULN = "1001234567";

            var result = _validator.Validate(_validModel);

            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("abc123")]
        [TestCase("123456789")]
        [TestCase(" ")]
        public void ULNThatIsNotNumericOr10DigitsInLengthIsIvalid(string uln)
        {
            _validModel.ULN = uln;

            var result = _validator.Validate(_validModel);

            Assert.That(result.IsValid, Is.False);
        }

        public void ULNThatStartsWithAZeroIsInvalid()
        {
            _validModel.ULN = "0123456789";

            var result = _validator.Validate(_validModel);

            Assert.That(result.IsValid, Is.False);
        }

        [TestCase("1324")]
        [TestCase("123")]
        [TestCase("1")]
        public void CostIsWholeNumberGreaterThanZeroIsValid(string cost)
        {
            _validModel.Cost = cost;

            var result = _validator.Validate(_validModel);

            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("123.12")]
        [TestCase("123.1")]
        [TestCase("123.0")]
        [TestCase("fdsfdfd")]
        [TestCase("123.000")]
        public void CostNotNumericOrIsNotAWholeNumber(string cost)
        {
            _validModel.Cost = cost;

            var result = _validator.Validate(_validModel);

            Assert.That(result.IsValid, Is.False);
        }

        [TestCase("0")]
        [TestCase("-0")]
        [TestCase("-123.12")]
        [TestCase("-123")]
        [TestCase("-123.1232")]
        [TestCase("-0.001")]
        public void CostThatIsZeroOrNegativeNumberIsInvalid(string cost)
        {
            _validModel.Cost = cost;

            var result = _validator.Validate(_validModel);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void CostContainingValidCommaSeparatorIsValid()
        {
            _validModel.Cost = "1,234";

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(",111")]
        [TestCase("1,22")]
        [TestCase("1,22,222")]
        public void CostThatContainsBadlyFormatedCommaSeparatorsIsInvalid(string cost)
        {
            _validModel.Cost = cost;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void FirstAndLastNameIsEmpty()
        {
            _validModel.FirstName = "";
            _validModel.LastName = "";

            var result = _validator.Validate(_validModel);

            Assert.That(result.Errors.Any(m => m.ErrorMessage.Contains("Enter a first name")), Is.True);
            Assert.That(result.Errors.Any(m => m.ErrorMessage.Contains("Enter a last name")), Is.True);
            Assert.That(result.IsValid, Is.False);
        }

        #region DateOfBirth

        [TestCase(31, 2, 13, "Enter a valid date of birth")]
        //[TestCase(31, 12, 1899, "Enter a valid date of birth")]
        [TestCase(5, null, 1998, "Enter a valid date of birth")]
        [TestCase(5, 9, null, "Enter a valid date of birth")]
        [TestCase(null, 9, 1998, "Enter a valid date of birth")]
        [TestCase(5, 9, -1, "Enter a valid date of birth")]
        [TestCase(0, 0, 0, "Enter a valid date of birth")]
        [TestCase(1, 18, 1998, "Enter a valid date of birth")]
        public void ShouldFailValidationOnDateOfBirth(int? day, int? month, int? year, string expected)
        {
            _validModel.DateOfBirth = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(null, null, null)]
        [TestCase(5, 9, 1998)]
        [TestCase(1, 1, 1900)]
        public void ShouldNotFailValidationOnDateOfBirth(int? day, int? month, int? year)
        {
            _validModel.DateOfBirth = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldFailValidationOnDateOfBirthWithTodayAsValue()
        {
            var date = DateTime.Now;
            _validModel.DateOfBirth = new DateTimeViewModel(date.Day, date.Month, date.Year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The date of birth must be in the past");
        }

        #endregion

        #region StartDate

        [TestCase(31, 2, 2121, "Enter a valid start date")]
        [TestCase(5, null, 2121, "Enter a valid start date")]
        [TestCase(5, 9, null, "Enter a valid start date")]
        [TestCase(5, 9, -1, "Enter a valid start date")]
        [TestCase(0, 0, 0, "Enter a valid start date")]
        [TestCase(1, 18, 2121, "Enter a valid start date")]
        //[TestCase(5, 9, 1998, "Learner start date must be in the future")]
        public void ShouldFailValidationForStartDateWith(int? day, int? month, int? year, string expected)
        {

            _validModel.StartDate = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(null, null, null)]
        [TestCase(5, 9, 2100)]
        [TestCase(1, 1, 2023)]
        [TestCase(null, 9, 2067)]
        public void ShouldNotFailValidationForStartDate(int? day, int? month, int? year)
        {
            _validModel.StartDate = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        #endregion

        #region PlannedEndDate

        [TestCase(31, 2, 2121, "Enter a valid training end date")]
        [TestCase(5, null, 2121, "Enter a valid training end date")]
        [TestCase(5, 9, null, "Enter a valid training end date")]
        [TestCase(5, 9, -1, "Enter a valid training end date")]
        [TestCase(0, 0, 0, "Enter a valid training end date")]
        [TestCase(1, 18, 2121, "Enter a valid training end date")]
        //[TestCase(5, 9, 1998, "Learner Planned end date must be in the future")]
        public void ShouldFailValidationForPlannedEndDateWith(int? day, int? month, int? year, string expected)
        {

            _validModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(null, null, null)]
        [TestCase(5, 9, 2100)]
        [TestCase(1, 1, 2023)]
        [TestCase(null, 9, 2067)]
        public void ShouldNotFailValidationForPlannedEndDate(int? day, int? month, int? year)
        {
            _validModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldFailIfStartDateIsAfterEndDate()
        {
            _validModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            _validModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The end date must be later than the start date");
        }

        [Test]
        public void ShouldNotFailIfStartDateIsNull()
        {
            _validModel.StartDate = null;
            _validModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldNotFailIfEndDateIsNull()
        {
            _validModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            _validModel.EndDate = null;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        #endregion
    }
}
