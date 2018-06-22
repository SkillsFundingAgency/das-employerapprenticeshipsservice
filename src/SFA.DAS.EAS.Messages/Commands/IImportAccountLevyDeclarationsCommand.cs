namespace SFA.DAS.EAS.Messages.Commands
{
    public interface IImportAccountLevyDeclarationsCommand
    {
        long AccountId { get; set; }
        string PayeRef { get; set; }
    }
}
