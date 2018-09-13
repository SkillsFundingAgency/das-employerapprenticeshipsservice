using MediatR;

namespace SFA.DAS.EmployerFinance.Events.ProcessDeclaration
{
    public class ProcessDeclarationsEvent : IAsyncNotification
    {
        public long AccountId { get; set; }
        public string EmpRef { get; set; }
    }
}
