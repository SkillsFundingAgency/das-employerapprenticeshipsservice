using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.EAS.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingCost : ApprenticeshipValidationTestBase
    {
        [TestCase("1000")]
        [TestCase("1234")]
        [TestCase("123")]
        [TestCase("1")]
        public void CostIsWholeNumberGreaterThanZeroIsValid(string cost)
        {
            ValidModel.Cost = cost;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase("123.12")]
        [TestCase("123.1")]
        [TestCase("123.0")]
        [TestCase("fdsfdfd")]
        [TestCase("123.000")]
        public void CostNotNumericOrIsNotAWholeNumber(string cost)
        {
            ValidModel.Cost = cost;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("0")]
        [TestCase("-0")]
        [TestCase("-123.12")]
        [TestCase("-123")]
        [TestCase("-123.1232")]
        [TestCase("-0.001")]
        public void CostThatIsZeroOrNegativeNumberIsInvalid(string cost)
        {
            ValidModel.Cost = cost;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("1234567")]
        public void CostMustContain6DigitsOrLess(string value)
        {
            ValidModel.Cost = value;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("100,000")]
        public void CostMustContain6DigitsOrLessIgnoringCommas(string value)
        {
            ValidModel.Cost = value;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        public void CostContainingValidCommaSeparatorIsValid()
        {
            ValidModel.Cost = "1,234";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(",111")]
        [TestCase("1,22")]
        [TestCase("122,22")]
        [TestCase("12222,")]
        public void CostThatContainsBadlyFormatedCommaSeparatorsIsInvalid(string cost)
        {
            ValidModel.Cost = cost;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void CostCannotBeOver100000()
        {
            ValidModel.Cost = "100001";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void VeryLargeNumberShouldNotCauseOverflow()
        {
            ValidModel.Cost = "999999999999999999999999999999";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }
    }
}
