using System;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ApproveApprenticeship;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.ApproveApprenticeshipCommandTests
{
    [TestFixture]
    public sealed class WhenValidatingTheApproveApprenticeshipCommand
    {
        private ApproveApprenticeshipCommandValidator _validator;
        private ApproveApprenticeshipCommand _validCommand;

        [SetUp]
        public void Arrange()
        {
            _validator = new ApproveApprenticeshipCommandValidator();

            _validCommand = new ApproveApprenticeshipCommand
            {
                EmployerAccountId = 1L,
                CommitmentId = 2L,
                ApprenticeshipId = 3L
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
            _validCommand.CommitmentId = commitmentId;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }

        [TestCase(0)]
        [TestCase(-3)]
        public void WhenApprenticeshipIdIsNotPositiveNumberIsInvalid(long apprenticeshipId)
        {
            _validCommand.ApprenticeshipId = apprenticeshipId;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }
    }
}
