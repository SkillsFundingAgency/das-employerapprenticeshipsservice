namespace SFA.DAS.EmployerFinance.Messages
{
    public interface IAccountMessage
    {
        string AccountHashedId { get; set; }
        long? AccountId { get; set; }
    }
}