using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerAccounts.Commands.SendNotification;

public class SendNotificationCommand : IAsyncRequest
{
    public Email Email { get; set; }
}