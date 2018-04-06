using MediatR;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetTransferTransactionDetails
{
    public class GetTransferTransactionDetailsQueryHandler : IAsyncRequestHandler<GetTransferTransactionDetailsQuery, GetTransferTransactionDetailsResponse>
    {
        private readonly EmployerFinancialDbContext _dbContext;
        private readonly IPublicHashingService _publicHashingService;

        public GetTransferTransactionDetailsQueryHandler(EmployerFinancialDbContext dbContext,
            IPublicHashingService publicHashingService)
        {
            _dbContext = dbContext;
            _publicHashingService = publicHashingService;
        }

        public async Task<GetTransferTransactionDetailsResponse> Handle(GetTransferTransactionDetailsQuery query)
        {
            var targetAccountId = _publicHashingService.DecodeValue(query.TargetAccountPublicHashedId);

            var result = await _dbContext.GetTransfersByTargetAccountId(
                                    query.AccountId.GetValueOrDefault(),
                                    targetAccountId,
                                    query.PeriodEnd);

            var transfers = result as AccountTransfer[] ?? result.ToArray();

            var firstTransfer = transfers.First();

            var senderAccountName = firstTransfer.SenderAccountName;
            var senderPublicHashedAccountId = _publicHashingService.HashValue(firstTransfer.SenderAccountId);

            var receiverAccountName = firstTransfer.ReceiverAccountName;
            var receiverPublicHashedAccountId = _publicHashingService.HashValue(firstTransfer.ReceiverAccountId);

            var courseTransfers = transfers.GroupBy(t => t.CourseName);

            var transferDetails = courseTransfers.Select(ct => new AccountTransferDetails
            {
                CourseName = ct.First().CourseName,
                PaymentTotal = ct.Sum(t => t.Amount),
                ApprenticeCount = (uint)ct.DistinctBy(t => t.ApprenticeshipId).Count()
            }).ToArray();

            var transferDate = transfers.FirstOrDefault()?.TransferDate ?? default(DateTime);
            var transfersPaymentTotal = transferDetails.Sum(t => t.PaymentTotal);

            var currentAccountPublicHashedId = _publicHashingService.HashValue(query.AccountId.GetValueOrDefault());

            return new GetTransferTransactionDetailsResponse
            {
                SenderAccountName = senderAccountName,
                SenderAccountPublicHashedId = senderPublicHashedAccountId,
                ReceiverAccountName = receiverAccountName,
                ReceiverAccountPublicHashedId = receiverPublicHashedAccountId,
                IsCurrentAccountSender = currentAccountPublicHashedId.Equals(senderPublicHashedAccountId),
                TransferDetails = transferDetails,
                TransferPaymentTotal = transfersPaymentTotal,
                DateCreated = transferDate
            };
        }
    }
}