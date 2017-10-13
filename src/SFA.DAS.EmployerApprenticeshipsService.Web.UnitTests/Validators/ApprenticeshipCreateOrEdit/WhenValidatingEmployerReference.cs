using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.EAS.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingEmployerReference : ApprenticeshipValidationTestBase
    {
        [TestCase("")]
        [TestCase(null)]
        public void EmployerReferenceIsOptional(string reference)
        {
            ValidModel.EmployerRef = reference;

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(0);
        }

        [Test]
        public void EmployerReferenceCanBeAMaximumOf20Characters()
        {
            ValidModel.EmployerRef = new string('*', 21);

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("The Reference must be 20 characters or fewer");
            result.Errors[0].ErrorCode.Should().Be("EmployerRef_01");
        }

        [Test]
        public void EmployerReferenceLessCanBeLessThan20Characters()
        {
            ValidModel.EmployerRef = "A valid reference";

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(0);
        }
    }
}
