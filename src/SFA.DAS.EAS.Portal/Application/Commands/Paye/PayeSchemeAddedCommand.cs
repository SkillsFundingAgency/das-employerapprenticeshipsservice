using System;

namespace SFA.DAS.EAS.Portal.Application.Commands.Paye
{
    public class PayeSchemeAddedCommand : ICommand
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
        public string PayeRef { get; set; }
        public DateTime Created { get; set; }

        public PayeSchemeAddedCommand(long accountId, string userName, Guid userRef, string payeRef, DateTime created)
        {
            AccountId = accountId;
            UserName = userName;
            UserRef = userRef;
            PayeRef = payeRef;
            Created = created;
        }
    }
}
