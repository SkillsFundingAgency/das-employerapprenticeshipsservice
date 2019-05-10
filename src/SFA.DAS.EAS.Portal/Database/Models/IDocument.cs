namespace SFA.DAS.EAS.Portal.Database.Models
{
    public interface IDocument
    {
        string Id { get; }
        string ETag { get; }
    }
}