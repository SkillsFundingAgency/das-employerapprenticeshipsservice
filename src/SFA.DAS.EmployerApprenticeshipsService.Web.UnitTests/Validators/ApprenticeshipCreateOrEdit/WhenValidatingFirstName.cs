using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.EAS.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingFirstName : ApprenticeshipValidationTestBase
    {

        [Test]
        public void TestFirstNamesNotNull()
        {
            ValidModel.FirstName = null;

            var result = Validator.Validate(ValidModel);
            result.Errors.Count.Should().Be(1);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("First name must be entered");
        }

        [Test]
        public void FirstNameShouldNotBeEmpty()
        {
            ValidModel.FirstName = " ";

            var result = Validator.Validate(ValidModel);
            result.Errors.Count.Should().Be(1);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("First name must be entered");
        }

        [TestCase(99, 0)]
        [TestCase(100, 0)]
        [TestCase(101, 1)]
        public void TestLengthOfFirstName(int length, int expectedErrorCount)
        {
            ValidModel.FirstName = new string('*', length);

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(expectedErrorCount);

            if (expectedErrorCount > 0)
            {
                result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter a first name that's no longer than 100 characters");
            }
        }
    }
}