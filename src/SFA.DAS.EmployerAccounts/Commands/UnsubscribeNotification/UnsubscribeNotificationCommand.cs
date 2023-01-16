namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification;

public class UnsubscribeNotificationCommand : IRequest
{
    public string UserRef { get; set; }

    public long AccountId { get; set; }
}