namespace SFA.DAS.EAS.Application.Messages
{
    public interface IAccountMessage
    {
        long? AccountId { get; set; }
        string AccountHashedId { get; set; }
    }
}