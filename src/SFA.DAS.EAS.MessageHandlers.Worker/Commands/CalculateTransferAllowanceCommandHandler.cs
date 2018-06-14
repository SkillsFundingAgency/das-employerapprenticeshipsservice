using System;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Interfaces;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.MessageHandlers.Worker.Commands
{
    [TopicSubscription("MA_CalculateTransferAllowanceSnapshotCommand")]
    public class CalculateTransferAllowanceCommandHandler : MessageProcessor<CalculateTransferAllowanceSnapshotCommand>
    {
        private readonly EmployerFinancialDbContext _db;
        private readonly ILog _log;
        private readonly IDateService _datetimeService;
        private readonly IMediator _mediator;

        public CalculateTransferAllowanceCommandHandler(
            IMessageSubscriberFactory subscriberFactory, 
            ILog log,
            EmployerFinancialDbContext dbContext,
            IDateService datetimeService,
            IMediator mediator) : base(subscriberFactory, log)
        {
            _db = dbContext;
            _log = log;
            _datetimeService = datetimeService;
            _mediator = mediator;
        }

        protected override Task ProcessMessage(CalculateTransferAllowanceSnapshotCommand messageContent)
        {
            var transferAllowanceResponse = _mediator.SendAsync(new GetTransferAllowanceQuery {AccountId = messageContent.AccountId});
            var transfer = new AccountTransferAllowanceSnapshot
            {
                AccountId = messageContent.AccountId,
                Year = _datetimeService.CurrentFinancialYear.EndYear,
                SnapshotTime = DateTime.UtcNow,
                TransferAllowance = transferAllowanceResponse.Result.TransferAllowance
            };
            return Upsert(transfer);
        }

        private async Task Upsert(AccountTransferAllowanceSnapshot transferAllowance)
        {
            if(!await Insert(transferAllowance))
            {
                await Update(transferAllowance);
            }
        }

        private async Task<bool> Insert(AccountTransferAllowanceSnapshot transferAllowance)
        {
            try
            {
                _db.AccountTransferSnapshots.Add(transferAllowance);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (UpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException && sqlException.Errors.OfType<SqlError>()
                        .Any(se => se.Number == 2601 || se.Number == 2627 /* PK/UKC violation */))
                {
                    _db.AccountTransferSnapshots.Remove(transferAllowance);
                    return false;
                }
                throw;
            }
        }

        private async Task<bool> Update(AccountTransferAllowanceSnapshot transferAllowance)
        {
            var existingTransferSnapshot = _db.AccountTransferSnapshots
                                                .Single(ts => ts.AccountId == transferAllowance.AccountId && ts.Year == transferAllowance.Year);

            existingTransferSnapshot.TransferAllowance = transferAllowance.TransferAllowance;
            existingTransferSnapshot.SnapshotTime = transferAllowance.SnapshotTime;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
