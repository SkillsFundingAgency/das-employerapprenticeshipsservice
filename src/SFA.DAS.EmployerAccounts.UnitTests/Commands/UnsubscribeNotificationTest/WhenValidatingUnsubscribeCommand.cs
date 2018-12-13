using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.UnsubscribeNotificationTest
{
    [TestFixture]
    public  class WhenValidatingUnsubscribeCommand
    {
        private UnsubscribeNotificationValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new UnsubscribeNotificationValidator();
        }

        [TestCase("")]
        [TestCase(null)]
        public void ThenUserIdIsNull(string userId)
        {
            var command = new UnsubscribeNotificationCommand { UserRef = userId, AccountId = 123456 };
            var result = _sut.Validate(command);

            result.IsValid().Should().BeFalse();
        }

        [TestCase(0)]
        [TestCase(-2)]
        public void AccountIdIsLessThan1(long accountId)
        {
            var command = new UnsubscribeNotificationCommand { UserRef = "ABBA123", AccountId = accountId };
            var result = _sut.Validate(command);

            result.IsValid().Should().BeFalse();
        }

        public void CommandIsValid()
        {
            var command = new UnsubscribeNotificationCommand { UserRef = "ABBA123", AccountId = 123456 };
            var result = _sut.Validate(command);

            result.IsValid().Should().BeTrue();
        }
    }
}
