namespace SFA.DAS.EAS.Portal.Application.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public abstract string MessageId { get; protected set; }
    }
}
