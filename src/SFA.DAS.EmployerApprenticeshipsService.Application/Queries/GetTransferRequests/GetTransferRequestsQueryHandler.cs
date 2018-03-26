using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.Queries.GetTransferRequests
{
    public class GetTransferRequestsQueryHandler : IAsyncRequestHandler<GetTransferRequestsQuery, GetTransferRequestsResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetTransferRequestsQueryHandler(EmployerAccountDbContext db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetTransferRequestsResponse> Handle(GetTransferRequestsQuery message)
        {
            var transferRequests = await _db.TransferRequests
                .Where(r => r.SenderAccount.Id == message.AccountId || r.ReceiverAccount.Id == message.AccountId)
                .OrderBy(r => r.CreatedDate)
                .ProjectTo<TransferRequestDto>(_configurationProvider)
                .ToListAsync();

            return new GetTransferRequestsResponse
            {
                AccountId = message.AccountId.Value,
                TransferRequests = transferRequests
            };
        }
    }
}