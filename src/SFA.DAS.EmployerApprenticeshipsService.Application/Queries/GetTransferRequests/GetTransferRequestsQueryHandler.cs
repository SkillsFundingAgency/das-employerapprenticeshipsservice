using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetTransferRequests
{
    public class GetTransferRequestsQueryHandler : IAsyncRequestHandler<GetTransferRequestsQuery, GetTransferRequestsResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IEmployerCommitmentApi _employerCommitmentApi;
        private readonly IHashingService _hashingService;

        public GetTransferRequestsQueryHandler(
            EmployerAccountDbContext db,
            IConfigurationProvider configurationProvider,
            IEmployerCommitmentApi employerCommitmentApi,
            IHashingService hashingService)
        {
            _db = db;
            _configurationProvider = configurationProvider;
            _employerCommitmentApi = employerCommitmentApi;
            _hashingService = hashingService;
        }

        public async Task<GetTransferRequestsResponse> Handle(GetTransferRequestsQuery message)
        {
            var transferRequests = await _employerCommitmentApi.GetTransferRequests(message.AccountHashedId);

            var accountIds = transferRequests
                .SelectMany(r => new [] { r.HashedSendingEmployerAccountId, r.HashedReceivingEmployerAccountId })
                .Select(h => _hashingService.DecodeValue(h))
                .ToList();

            var accounts = await _db.Accounts
                .Where(a => accountIds.Contains(a.Id))
                .ProjectTo<AccountDto>(_configurationProvider)
                .ToDictionaryAsync(a => a.HashedId);

            var transferRequestsData = transferRequests
                .Select(r => new TransferRequestDto
                {
                    CreatedDate = r.CreatedOn,
                    ReceiverAccount = accounts[r.HashedReceivingEmployerAccountId],
                    SenderAccount = accounts[r.HashedSendingEmployerAccountId],
                    Status = r.Status,
                    TransferCost = r.TransferCost,
                    TransferRequestHashedId = r.HashedTransferRequestId
                })
                .OrderBy(r => r.ReceiverAccount.Id == message.AccountId.Value ? r.SenderAccount.Name : r.ReceiverAccount.Name)
                .ThenBy(r => r.CreatedDate)
                .ToList();

            return new GetTransferRequestsResponse
            {
                TransferRequests = transferRequestsData
            };
        }
    }
}