namespace SFA.DAS.EmployerAccounts.Messages
{
    public interface IAccountMessage
    {
        string AccountHashedId { get; set; }
        long? AccountId { get; set; }
    }
}