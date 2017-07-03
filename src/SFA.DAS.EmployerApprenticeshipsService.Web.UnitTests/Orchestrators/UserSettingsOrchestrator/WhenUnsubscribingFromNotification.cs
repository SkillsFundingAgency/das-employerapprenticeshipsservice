﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Settings;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.UserSettingsOrchestrator
{
    [TestFixture]
    public class WhenUnsubscribingFromNotification
    {
        private Web.Orchestrators.UserSettingsOrchestrator _sut;

        private Mock<IMediator> _mediator;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<IMediator>();
            _hashingService = new Mock<IHashingService>();

            _hashingService.Setup(m => m.DecodeValue("ABBA777")).Returns(777);
            _hashingService.Setup(m => m.DecodeValue("ABBA888")).Returns(888);
            _hashingService.Setup(m => m.DecodeValue("ABBA999")).Returns(999);
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetUserNotificationSettingsQuery>()))
                   .ReturnsAsync(new GetUserNotificationSettingsQueryResponse
                   {
                       NotificationSettings = new List<UserNotificationSetting>
                                                      {
                                                           new UserNotificationSetting
                                                               {
                                                                   AccountId = 999,
                                                                   HashedAccountId = "ABBA999",
                                                                   Id = 10999,
                                                                   Name = "Super Account 999",
                                                                   ReceiveNotifications = true,
                                                                   UserId = 110099
                                                               },
                                                           new UserNotificationSetting
                                                               {
                                                                   AccountId = 888,
                                                                   HashedAccountId = "ABBA888",
                                                                   Id = 10888,
                                                                   Name = "Super Account 888",
                                                                   ReceiveNotifications = false,
                                                                   UserId = 110088
                                                               }
                                                      }
                   });
            _sut = new Web.Orchestrators.UserSettingsOrchestrator(_mediator.Object, _hashingService.Object, Mock.Of<ILog>());
        }

        [Test]
        public void CantFindSettingsForAccount()
        {
            Func<Task> act = async () => await _sut.Unsubscribe("REF", "ABBA777", "URL/to/Settings");
            act.ShouldThrow<Exception>()
                .Where(m => m.Message == "Cannot find user settings for user REF in account 777");
        }

        [Test]
        public async Task WhenUserAlreadyUnsubscribe()
        {
            var result = await _sut.Unsubscribe("REF", "ABBA888", "URL/to/Settings");
            result.Data.AccountName.Should().Be("Super Account 888");
            result.Data.AlreadyUnsubscribed.Should().BeTrue();
        }

        [Test]
        public async Task WhenUserUnsubscribe()
        {
            var result = await _sut.Unsubscribe("REF", "ABBA999", "URL/to/Settings");
            result.Data.AccountName.Should().Be("Super Account 999");
            result.Data.AlreadyUnsubscribed.Should().BeFalse();
        }
    }
}
