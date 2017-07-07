using MediatR;

namespace SFA.DAS.EAS.Application.Events.ProcessDeclaration
{
    public class ProcessDeclarationsEvent : IAsyncNotification
    {
        public long AccountId { get; set; }
        public string EmpRef { get; set; }
    }
}
