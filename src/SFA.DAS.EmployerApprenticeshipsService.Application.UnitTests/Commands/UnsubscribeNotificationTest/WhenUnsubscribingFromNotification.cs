using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.EAS.Application.Commands.UnsubscribeNotification;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Settings;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UnsubscribeNotificationTest
{
    [TestFixture]
    public class WhenUnsubscribingFromNotification
    {
        private UnsubscribeNotificationHandler _sut;
        private Mock<IValidator<UnsubscribeNotificationCommand>> _mockValidator;
        private UnsubscribeNotificationCommand _command;
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IUserRepository> _userRepo;

        private Mock<INotificationsApi> _notiApi;

        [SetUp]
        public void SetUp()
        {
            _command = new UnsubscribeNotificationCommand
                           {
                               UserRef = "ABBA12",
                               AccountId = 123456
                           };

            _mockValidator = new Mock<IValidator<UnsubscribeNotificationCommand>>();
            _notiApi = new Mock<INotificationsApi>();
            _userRepo = new Mock<IUserRepository>();
            _accountRepository = new Mock<IAccountRepository>();

            _userRepo.Setup(m => m.GetByUserRef(_command.UserRef))
                .ReturnsAsync(new User
                                  {
                                      FirstName = "First name",
                                      LastName = "Last name",
                                      Email = "email@email.com",
                                      Id = 99L,
                                      UserRef = _command.UserRef
                                  });

            _accountRepository.Setup(m => m.GetUserAccountSettings(_command.UserRef))
                .ReturnsAsync(new List<UserNotificationSetting>
                                  {
                                      new UserNotificationSetting
                                          {
                                              AccountId = _command.AccountId,
                                              HashedAccountId = "ABBA12",
                                              Id = 123456L,
                                              Name = "Account Name",
                                              ReceiveNotifications = true
                                          }
                                  });

            _sut = new UnsubscribeNotificationHandler(
                _mockValidator.Object, 
                _notiApi.Object, 
                _userRepo.Object, 
                _accountRepository.Object, 
                Mock.Of<ILog>());
        }

        [Test]
        public void ShouldThrowExceptionIfAccountIsAlreadyUnsubscribed()
        {
            _accountRepository.Setup(m => m.GetUserAccountSettings(_command.UserRef))
                .ReturnsAsync(new List<UserNotificationSetting>
                                  {
                                      new UserNotificationSetting
                                          {
                                              AccountId = _command.AccountId,
                                              HashedAccountId = "ABBA12",
                                              Id = 123456L,
                                              Name = "Account Name",
                                              ReceiveNotifications = false
                                          }
                                  });
            Func<Task>  act =  async () => await _sut.Handle(_command);
            act.ShouldThrow<Exception>().Where(m => m.Message.StartsWith("Trying to unsubscribe from an already unsubscribed account"));
        }

        [Test]
        public void ShouldThrowExceptionIfAccountIsMissingSettings()
        {
            _accountRepository.Setup(m => m.GetUserAccountSettings(_command.UserRef))
                .ReturnsAsync(new List<UserNotificationSetting>
                                  {
                                      new UserNotificationSetting
                                          {
                                              AccountId = _command.AccountId +1,
                                              HashedAccountId = "ABBA13",
                                              Id = 999L,
                                              Name = "Account Name 2",
                                              ReceiveNotifications = true
                                          }
                                  });
            Func<Task> act = async () => await _sut.Handle(_command);
            act.ShouldThrow<Exception>().Where(m => m.Message.StartsWith("Missing settings for account 123456 and user with ref ABBA12"));
        }

        [Test]
        public async Task ShouldValidateCommand()
        {
            await _sut.Handle(_command);

            _mockValidator.Verify(m => m.Validate(_command), Times.Once);
        }

        [Test]
        public async Task ShouldUnsubscribeFromOneAccount()
        {
            await _sut.Handle(_command);
        }

        [Test]
        public async Task ShouldSendEmail()
        {
            Email mask = null;
            _command.NotificationSettingUrl = "this/is/url";
            _notiApi.Setup(n => n.SendEmail(It.IsAny<Email>()))
                .Callback<Email>(m => mask = m)
                .Returns(Task.FromResult(1));
                
            await _sut.Handle(_command);
            mask.RecipientsAddress.Should().Be("email@email.com");
            mask.Subject.Should().Be("UnsubscribeSuccessful");
            mask.Tokens["name"].Should().Be("First name");
            mask.Tokens["account_name"].Should().Be("Account Name");
            mask.Tokens["link_notification_page"].Should().Be(_command.NotificationSettingUrl);
        }
    }
}
