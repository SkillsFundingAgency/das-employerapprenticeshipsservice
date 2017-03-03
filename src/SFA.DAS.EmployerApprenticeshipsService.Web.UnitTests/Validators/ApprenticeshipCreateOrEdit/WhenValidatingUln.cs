using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.EAS.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingUln : ApprenticeshipValidationTestBase
    {
        [TestCase("abc123")]
        [TestCase("123456789")]
        [TestCase(" ")]
        [TestCase("9999999999")]
        public void ULNThatIsNotNumericOr10DigitsInLengthIsInvalid(string uln)
        {
            ValidModel.ULN = uln;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }
        
        [Test]
        public void ULN9999999999IsNotValid()
        {
            ValidModel.ULN = "9999999999";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ULNThatStartsWithAZeroIsInvalid()
        {
            ValidModel.ULN = "0123456789";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ULNWithValidValueIsValid()
        {
            ValidModel.ULN = "1234567898";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }
    }
}
