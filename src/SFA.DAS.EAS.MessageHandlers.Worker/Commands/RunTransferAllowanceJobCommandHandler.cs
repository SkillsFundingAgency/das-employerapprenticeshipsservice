using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.MessageHandlers.Worker.Commands
{
    [TopicSubscription("MA_RunTransferAllowanceSnapshotJobCommand")]
    public class RunTransferAllowanceJobCommandHandler : MessageProcessor<RunTransferAllowanceSnapshotJobCommand>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly ILog _log;
        private readonly IMessagePublisher _messagePublisher;

        public RunTransferAllowanceJobCommandHandler(
            IMessageSubscriberFactory subscriberFactory, 
            ILog log, 
            EmployerAccountDbContext dbContext, 
            IMessagePublisher messagePublisher,
            IMessageContextProvider messageContextProvider) : base(subscriberFactory, log, messageContextProvider)
        {
            _db = dbContext;
            _log = log;
            _messagePublisher = messagePublisher;
        }

        protected override Task ProcessMessage(RunTransferAllowanceSnapshotJobCommand messageContent)
        {
            var accountIdsToPublish = new BlockingCollection<long>();
            const int numberOfReaders = 4;
            int accountsFound = 0;

            var queueProcessorTask = StartQueueProcessor(accountIdsToPublish, numberOfReaders);

            foreach (var accountId in _db.Accounts.Select(ac => ac.Id))
            {
                accountIdsToPublish.Add(accountId);
                accountsFound++;
            }

            accountIdsToPublish.CompleteAdding();

            _log.Info($"Found {accountsFound} accounts and processing using {numberOfReaders} workers");

            return queueProcessorTask;
        }

        private Task StartQueueProcessor(BlockingCollection<long> accountIdsToPublish, int workers)
        {
            var queueReaderTasks = new Task<int>[workers];

            for (int i = 0; i < workers; i++)
            {
                queueReaderTasks[i] = StartQueueReaderTask(accountIdsToPublish);
            }

            return Task
                    .WhenAll(queueReaderTasks)
                    .ContinueWith(i =>  LogTaskProcessingNumbers(queueReaderTasks));
        }

        private Task<int> StartQueueReaderTask(BlockingCollection<long> accountIdsToPublish)
        {
            return Task.Run(() =>
            {
                _log.Debug($"Starting worker thread");
                int processedCount = 0;
                while (!accountIdsToPublish.IsCompleted)
                {
                    if (accountIdsToPublish.TryTake(out long accountId, 100))
                    {
                        _messagePublisher.PublishAsync(
                            new CalculateTransferAllowanceSnapshotCommand { AccountId = accountId });
                        processedCount++;
                    }
                }

                _log.Debug($"Stopped worker thread - processed {processedCount}");
                return processedCount;
            });
        }

        private void LogTaskProcessingNumbers(Task<int>[] tasks)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Number of reader tasks:");
            sb.Append(tasks.Length);
            sb.AppendLine();

            for (int i = 0; i < tasks.Length; i++)
            {
                sb.Append("task ");
                sb.Append(i);
                sb.Append(":");
                if (tasks[i].IsFaulted)
                {
                    sb.Append("Failed -");
                    sb.Append(tasks[i].Exception.GetMessage());
                    sb.AppendLine();
                }
                else
                {
                    sb.Append(tasks[i].Result);
                    sb.AppendLine();
                }
            }
       }
    }
}
