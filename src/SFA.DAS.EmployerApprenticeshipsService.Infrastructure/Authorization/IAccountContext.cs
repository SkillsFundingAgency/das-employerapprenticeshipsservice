namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public interface IAccountContext
    {
        long Id { get; }
        string HashedId { get; }
        string PublicHashedId { get; }
    }
}