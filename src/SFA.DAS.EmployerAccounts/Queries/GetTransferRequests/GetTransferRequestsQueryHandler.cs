﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferRequests
{
    public class GetTransferRequestsQueryHandler : IAsyncRequestHandler<GetTransferRequestsQuery, GetTransferRequestsResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ICommitmentsV2ApiClient _employerCommitmentApi;
        private readonly IHashingService _hashingService;        

        public GetTransferRequestsQueryHandler(
            Lazy<EmployerAccountsDbContext> db,
            IConfigurationProvider configurationProvider,
            ICommitmentsV2ApiClient employerCommitmentApi,
            IHashingService hashingService)
        {
            _db = db;
            _configurationProvider = configurationProvider;
            _employerCommitmentApi = employerCommitmentApi;
            _hashingService = hashingService;
        }

        public async Task<GetTransferRequestsResponse> Handle(GetTransferRequestsQuery message)
        {
            var accountHashedId = _hashingService.HashValue(message.AccountId);
            var transferRequests = await _employerCommitmentApi.GetTransferRequests(message.AccountId);

            var accountIds = transferRequests.TransferRequestSummaryResponse
                .SelectMany(r => new[] { r.HashedSendingEmployerAccountId, r.HashedReceivingEmployerAccountId })
                .Select(h => _hashingService.DecodeValue(h))
                .ToList();

            var accounts = await _db.Value.Accounts
                .Where(a => accountIds.Contains(a.Id))
                .ProjectTo<AccountDto>(_configurationProvider)
                .ToDictionaryAsync(a => a.HashedId);

            var transferRequestsData = transferRequests.TransferRequestSummaryResponse
                .Select(r => new TransferRequestDto
                {
                    CreatedDate = r.CreatedOn,
                    ReceiverAccount = accounts[r.HashedReceivingEmployerAccountId],
                    SenderAccount = accounts[r.HashedSendingEmployerAccountId],
                    Status = r.Status,
                    TransferCost = r.TransferCost,
                    TransferRequestHashedId = r.HashedTransferRequestId
                })
                .OrderBy(r => r.ReceiverAccount.Id == message.AccountId ? r.SenderAccount.Name : r.ReceiverAccount.Name)
                .ThenBy(r => r.CreatedDate)
                .ToList();

            return new GetTransferRequestsResponse
            {
                TransferRequests = transferRequestsData,
                AccountId = message.AccountId
            };
        }
    }
}