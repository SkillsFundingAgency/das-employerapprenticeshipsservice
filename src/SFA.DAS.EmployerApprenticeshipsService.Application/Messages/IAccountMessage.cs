namespace SFA.DAS.EAS.Application.Messages
{
    public interface IAccountMessage
    {
        string AccountHashedId { get; set; }
        long? AccountId { get; set; }
    }
}