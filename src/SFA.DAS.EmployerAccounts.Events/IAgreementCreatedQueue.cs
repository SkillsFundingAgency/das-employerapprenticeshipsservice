using SFA.DAS.EAS.Domain.Attributes;

namespace SFA.DAS.EmployerAccounts.Events
{
    public interface IAgreementCreatedQueue
    {
        [QueueName]
         string agreement_created_messages { get; set; }
    }
}
