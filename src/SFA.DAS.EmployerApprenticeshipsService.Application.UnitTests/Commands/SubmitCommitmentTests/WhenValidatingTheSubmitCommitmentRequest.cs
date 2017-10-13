using System;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SubmitCommitment;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SubmitCommitmentTests
{
    [TestFixture]
    public class WhenValidatingTheSubmitCommitmentRequest
    {
        private SubmitCommitmentCommandValidator _validator;
        private SubmitCommitmentCommand _validCommand;

        [SetUp]
        public void Arrange()
        {
            _validator = new SubmitCommitmentCommandValidator();

            _validCommand = new SubmitCommitmentCommand
            {
                EmployerAccountId = 1L,
                CommitmentId = 2L
            };
        }

        [Test]
        public void WhenCommandIsNullIsInvalid()
        {
            Assert.Throws<ArgumentNullException>(() => _validator.Validate(null));
        }

        [TestCase(0)]
        [TestCase(-3)]
        public void WhenEmployerAccountIdIsNotPositiveNumberIsInvalid(long accoundId)
        {
            _validCommand.EmployerAccountId = accoundId;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }

        [TestCase(0)]
        [TestCase(-3)]
        public void WhenCommitmentIdIsNotPositiveNumberIsInvalid(long commitmentId)
        {
            _validCommand.CommitmentId= commitmentId;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WhenUserDisplayNameIsNotSetIsInvalid(string value)
        {
            _validCommand.UserDisplayName = value;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WhenUserEmailAddressIsNotSetIsInvalid(string value)
        {
            _validCommand.UserEmailAddress = value;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }
    }
}
