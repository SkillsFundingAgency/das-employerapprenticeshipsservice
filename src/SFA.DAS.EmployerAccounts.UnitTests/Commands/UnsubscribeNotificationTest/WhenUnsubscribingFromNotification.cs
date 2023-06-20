using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.Notifications.Api.Client;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.UnsubscribeNotificationTest
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

            _userRepo.Setup(m => m.GetUserByRef(_command.UserRef))
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

            _sut = new UnsubscribeNotificationHandler(_mockValidator.Object, _accountRepository.Object);
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
            Func<Task> act = async () => await _sut.Handle(_command, CancellationToken.None);
            act.Should().ThrowAsync<Exception>().Where(m => m.Message.StartsWith("Trying to unsubscribe from an already unsubscribed account"));
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
            Func<Task> act = async () => await _sut.Handle(_command, CancellationToken.None);
            act.Should().ThrowAsync<Exception>().Where(m => m.Message.StartsWith("Missing settings for account 123456 and user with ref ABBA12"));
        }

        [Test]
        public async Task ShouldValidateCommand()
        {
            await _sut.Handle(_command, CancellationToken.None);

            _mockValidator.Verify(m => m.Validate(_command), Times.Once);
        }

        [Test]
        public async Task ShouldUnsubscribeFromOneAccount()
        {
            List<UserNotificationSetting> list = null;
            _accountRepository.Setup(m => m.UpdateUserAccountSettings(It.IsAny<string>(), It.IsAny<List<UserNotificationSetting>>()))
                .Returns(Task.FromResult(1L))
               .Callback<string, List<UserNotificationSetting>>((m, l) => list = l);

            await _sut.Handle(_command, CancellationToken.None);

            list.Should().NotBeNull();
            var setting = list.SingleOrDefault(m => m.AccountId == _command.AccountId);
            setting.ReceiveNotifications.Should().BeFalse();
        }
    }
}
