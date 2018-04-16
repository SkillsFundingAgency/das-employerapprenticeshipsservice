using MediatR;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetTransferSenderTransactionDetails
{
    public class GetTransferSenderTransactionDetailsQueryHandler : IAsyncRequestHandler<GetTransferSenderTransactionDetailsQuery, GetTransferSenderTransactionDetailsResponse>
    {
        private readonly EmployerFinancialDbContext _dbContext;
        private readonly IPublicHashingService _publicHashingService;

        public GetTransferSenderTransactionDetailsQueryHandler(EmployerFinancialDbContext dbContext,
            IPublicHashingService publicHashingService)
        {
            _dbContext = dbContext;
            _publicHashingService = publicHashingService;
        }

        public async Task<GetTransferSenderTransactionDetailsResponse> Handle(GetTransferSenderTransactionDetailsQuery query)
        {
            var result = await _dbContext.GetSenderTransfersByReceiver(
                                    query.AccountId.GetValueOrDefault(),
                                    query.ReceiverAccountId,
                                    query.PeriodEnd);

            var transfers = result as AccountTransfer[] ?? result.ToArray();

            var receiverAccountName = transfers.FirstOrDefault()?.ReceiverAccountName ?? string.Empty;
            var receiverPublicHashedAccountId = _publicHashingService.HashValue(query.ReceiverAccountId);

            var courseTransfers = transfers.GroupBy(t => t.CourseName);

            var transferDetails = courseTransfers.Select(ct => new AccountTransferDetails
            {
                CourseName = ct.First().CourseName,
                PaymentTotal = ct.Sum(t => t.Amount),
                ApprenticeCount = (uint)ct.DistinctBy(t => t.ApprenticeshipId).Count()
            }).ToArray();

            var transferDate = transfers.FirstOrDefault()?.TransferDate ?? default(DateTime);
            var transfersPaymentTotal = transferDetails.Sum(t => t.PaymentTotal);

            return new GetTransferSenderTransactionDetailsResponse
            {
                DateCreated = transferDate,
                ReceiverAccountName = receiverAccountName,
                TransferDetails = transferDetails,
                ReceiverPublicHashedId = receiverPublicHashedAccountId,
                TransferPaymentTotal = transfersPaymentTotal
            };
        }
    }
}