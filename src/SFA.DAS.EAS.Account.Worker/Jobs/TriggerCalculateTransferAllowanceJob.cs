using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Worker.Data;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.Jobs
{
    public class TriggerCalculateTransferAllowanceJob : IJob
    {
        private readonly IMessagePublisher _messagePublisher;

        public TriggerCalculateTransferAllowanceJob(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public Task Run()
        {
            return _messagePublisher.PublishAsync(new RunTransferAllowanceSnapshotJobCommand());
        }
    }
}
