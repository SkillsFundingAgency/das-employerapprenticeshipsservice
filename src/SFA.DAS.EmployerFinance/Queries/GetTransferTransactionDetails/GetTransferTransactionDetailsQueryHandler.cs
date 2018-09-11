﻿using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Models.Transfers;
using SFA.DAS.Hashing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferTransactionDetails
{
    public class GetTransferTransactionDetailsQueryHandler : IAsyncRequestHandler<GetTransferTransactionDetailsQuery, GetTransferTransactionDetailsResponse>
    {
        private readonly EmployerFinanceDbContext _dbContext;
        private readonly IPublicHashingService _publicHashingService;

        public GetTransferTransactionDetailsQueryHandler(EmployerFinanceDbContext dbContext,
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

            var transfers = result as List<AccountTransfer> ?? result.ToList();

            var firstTransfer = transfers.First();

            var senderAccountName = firstTransfer.SenderAccountName;
            var senderPublicHashedAccountId = _publicHashingService.HashValue(firstTransfer.SenderAccountId);

            var receiverAccountName = firstTransfer.ReceiverAccountName;
            var receiverPublicHashedAccountId = _publicHashingService.HashValue(firstTransfer.ReceiverAccountId);

            var courseTransfers = transfers.GroupBy(t => new { t.CourseName, t.CourseLevel });

            var transferDetails = courseTransfers.Select(ct => new AccountTransferDetails
            {
                CourseName = ct.First().CourseName,
                CourseLevel = ct.First().CourseLevel,
                PaymentTotal = ct.Sum(t => t.Amount),
                ApprenticeCount = (uint)ct.DistinctBy(t => t.CommitmentId).Count()
            }).ToArray();

            var periodEnd = _dbContext.PeriodEnds.Single(p => p.PeriodEndId.Equals(firstTransfer.PeriodEnd));

            var transferDate = periodEnd.CompletionDateTime;
            var transfersPaymentTotal = transferDetails.Sum(t => t.PaymentTotal);

            var isCurrentAccountSender = query.AccountId.GetValueOrDefault() == firstTransfer.SenderAccountId;

            return new GetTransferTransactionDetailsResponse
            {
                SenderAccountName = senderAccountName,
                SenderAccountPublicHashedId = senderPublicHashedAccountId,
                ReceiverAccountName = receiverAccountName,
                ReceiverAccountPublicHashedId = receiverPublicHashedAccountId,
                IsCurrentAccountSender = isCurrentAccountSender,
                TransferDetails = transferDetails,
                TransferPaymentTotal = transfersPaymentTotal,
                DateCreated = transferDate
            };
        }
    }
}