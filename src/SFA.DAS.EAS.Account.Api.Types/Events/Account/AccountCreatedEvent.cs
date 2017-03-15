namespace SFA.DAS.EAS.Account.Api.Types.Events.Account
{
    public class AccountCreatedEvent : IEventView
    {
        public long Id { get; set; }
        public string Event { get; set; }
        public string ResourceUri { get; set; }
    }
}
