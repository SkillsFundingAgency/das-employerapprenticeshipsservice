using System;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.UpdateApprenticeshipStatus;
using SFA.DAS.EAS.Domain.Models.Apprenticeship;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UpdateApprenticeshipStatusTests
{
    [TestFixture]
    public sealed class WhenValidatingTheUpdateApprenticeshipStatusCommand
    {
        private UpdateApprenticeshipStatusCommandValidator _validator;
        private UpdateApprenticeshipStatusCommand _validCommand;

        [SetUp]
        public void Arrange()
        {
            _validator = new UpdateApprenticeshipStatusCommandValidator();

            _validCommand = new UpdateApprenticeshipStatusCommand
            {
                EmployerAccountId = 1L,
                ApprenticeshipId = 3L,
                ChangeType = ChangeStatusType.Stop,
                DateOfChange = DateTime.UtcNow.Date,
                UserId = "user123"
            };
        }

        [Test]
        public void WhenValidCommandResultIsValid()
        {
            var result = _validator.Validate(_validCommand);

            Assert.IsTrue(result.IsValid());
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
        public void WhenApprenticeshipIdIsNotPositiveNumberIsInvalid(long apprenticeshipId)
        {
            _validCommand.ApprenticeshipId = apprenticeshipId;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }

        [TestCase(0)]
        [TestCase(5)]
        public void WhenTheChangeTypeIsNotValid(int invalidChangeType)
        {
            _validCommand.ChangeType = (ChangeStatusType)invalidChangeType;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }
    }
}
