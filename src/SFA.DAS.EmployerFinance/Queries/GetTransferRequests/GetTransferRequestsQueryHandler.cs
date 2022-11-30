using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferRequests
{
    public class GetTransferRequestsQueryHandler : IAsyncRequestHandler<GetTransferRequestsQuery, GetTransferRequestsResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountsRepository;
        private readonly IMapper _mapper;
        private readonly ICommitmentsV2ApiClient _commitmentV2ApiClient;
        private readonly IHashingService _hashingService;        

        public GetTransferRequestsQueryHandler(
            IEmployerAccountRepository employerAccountsRepository,
            IMapper mapper,
            ICommitmentsV2ApiClient commitmentsV2Apiclient,
            IHashingService hashingService)
        {
            _employerAccountsRepository = employerAccountsRepository;
            _mapper = mapper;
            _commitmentV2ApiClient = commitmentsV2Apiclient;
            _hashingService = hashingService;
        }

        public async Task<GetTransferRequestsResponse> Handle(GetTransferRequestsQuery message)
        {
            var accountHashedId = _hashingService.HashValue(message.AccountId);
            var transferRequests = await _commitmentV2ApiClient.GetTransferRequests(message.AccountId);

            var accountIds = transferRequests.TransferRequestSummaryResponse
                .SelectMany(r => new[] { r.HashedSendingEmployerAccountId, r.HashedReceivingEmployerAccountId })
                .Select(h => _hashingService.DecodeValue(h))
                .ToList();

            var accounts = _mapper.Map<List<AccountDto>>(await _employerAccountsRepository.Get(accountIds))
                .ToDictionary(p => p.HashedId);
            
            var transferRequestsData = transferRequests.TransferRequestSummaryResponse
                .Select(r => new TransferRequestDto
                {
                    CreatedDate = r.CreatedOn,
                    ReceiverAccount = accounts[r.HashedReceivingEmployerAccountId],
                    SenderAccount = accounts[r.HashedSendingEmployerAccountId],
                    Status = r.Status,
                    TransferCost = r.TransferCost,
                    TransferRequestHashedId = r.HashedTransferRequestId,
                    Type = message.AccountId == accounts[r.HashedSendingEmployerAccountId].Id ? TransferConnectionType.Sender : TransferConnectionType.Receiver
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