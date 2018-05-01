using System;
using FluentAssertions;

using NUnit.Framework;

using SFA.DAS.EAS.Application.Commands.UnsubscribeNotification;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UnsubscribeNotificationTest
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

        [TestCase]
        public void ThenUserIdIsGuidEmpty()
        {
            var command = new UnsubscribeNotificationCommand { ExternalUserId = Guid.Empty, AccountId = 123456 };
            var result = _sut.Validate(command);

            result.IsValid().Should().BeFalse();
        }

        [TestCase(0)]
        [TestCase(-2)]
        public void AccountIdIsLessThan1(long accountId)
        {
            var command = new UnsubscribeNotificationCommand { ExternalUserId = Guid.NewGuid(), AccountId = accountId };
            var result = _sut.Validate(command);

            result.IsValid().Should().BeFalse();
        }

        public void CommandIsValid()
        {
            var command = new UnsubscribeNotificationCommand { ExternalUserId = Guid.NewGuid(), AccountId = 123456 };
            var result = _sut.Validate(command);

            result.IsValid().Should().BeTrue();
        }
    }
}
