namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public interface IUserEvent : IEvent
    {
        string CreatorName { get; }
        string CreatorUserRef { get; }
    }
}