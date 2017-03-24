using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.EAS.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingLastName : ApprenticeshipValidationTestBase
    { 
        [Test]
        public void TestLastNameNotNull()
        {
            ValidModel.LastName = null;

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("Last name must be entered");
        }

        [Test]
        public void LastNameShouldNotBeEmpty()
        {
            ValidModel.LastName = " ";

            var result = Validator.Validate(ValidModel);
            result.Errors.Count.Should().Be(1);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("Last name must be entered");
        }

        [TestCase(99, 0)]
        [TestCase(100, 0)]
        [TestCase(101, 1)]
        public void TestLengthOfLastName(int length, int expectedErrorCount)
        {
            ValidModel.LastName = new string('*', length);

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(expectedErrorCount);

            if (expectedErrorCount > 0)
            {
                result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter a last name that's no longer than 100 characters");
            }
        }
    }
}
