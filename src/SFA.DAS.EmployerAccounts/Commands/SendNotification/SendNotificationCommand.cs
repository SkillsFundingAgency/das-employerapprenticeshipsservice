using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerAccounts.Commands.SendNotification;

public class SendNotificationCommand : IRequest
{
    public Email Email { get; set; }
}