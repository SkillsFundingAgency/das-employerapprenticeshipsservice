namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public interface IAccountContext
    {
        long Id { get; }
        string HashedId { get; }
        string PublicHashedId { get; }
    }
}