using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.UserSettingsOrchestrator;

[TestFixture]
public class WhenUnsubscribingFromNotification
{
    private Web.Orchestrators.UserSettingsOrchestrator _sut;

    private Mock<IMediator> _mediator;
    private Mock<IEncodingService> _endcodingService;

    [SetUp]
    public void SetUp()
    {
        _mediator = new Mock<IMediator>();
        _endcodingService = new Mock<IEncodingService>();

        _endcodingService.Setup(m => m.Decode("ABBA777", EncodingType.AccountId)).Returns(777);
        _endcodingService.Setup(m => m.Decode("ABBA888", EncodingType.AccountId)).Returns(888);
        _endcodingService.Setup(m => m.Decode("ABBA999", EncodingType.AccountId)).Returns(999);
        _mediator.Setup(m => m.Send(It.IsAny<GetUserNotificationSettingsQuery>(), It.IsAny<CancellationToken>()))
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

        _sut = new Web.Orchestrators.UserSettingsOrchestrator(_mediator.Object, Mock.Of<ILogger<Web.Orchestrators.UserSettingsOrchestrator>>(), _endcodingService.Object, null);
    }

    [Test]
    public void CantFindSettingsForAccount()
    {
        Func<Task> act = async () => await _sut.Unsubscribe("REF", "ABBA777");
        act.Should().ThrowAsync<Exception>()
            .Where(m => m.Message == "Cannot find user settings for user REF in account 777");
    }

    [Test]
    public async Task WhenUserAlreadyUnsubscribe()
    {
        var result = await _sut.Unsubscribe("REF", "ABBA888");
        result.Data.AccountName.Should().Be("Super Account 888");
        result.Data.AlreadyUnsubscribed.Should().BeTrue();
    }

    [Test]
    public async Task WhenUserUnsubscribe()
    {
        var result = await _sut.Unsubscribe("REF", "ABBA999");
        result.Data.AccountName.Should().Be("Super Account 999");
        result.Data.AlreadyUnsubscribed.Should().BeFalse();
    }
}