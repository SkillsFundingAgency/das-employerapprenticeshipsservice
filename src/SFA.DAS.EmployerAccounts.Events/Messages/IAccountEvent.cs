namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public interface IAccountEvent : IEvent
    {
        long AccountId { get; }
    }
}