using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTransferRole
{
    public class GetAccountTransferRoleHandler : IAsyncRequestHandler<GetAccountTransferRoleQuery, GetAccountTransferRoleResponse>
    {
        private readonly EmployerAccountDbContext _db;

        public GetAccountTransferRoleHandler(EmployerAccountDbContext db)
        {
            _db = db;
        }

        public async Task<GetAccountTransferRoleResponse> Handle(GetAccountTransferRoleQuery message)
        {
            var data = await _db.TransferConnectionInvitations
                .Where(i => i.ReceiverAccount.Id == message.AccountId)
                .GroupBy(i => i.ReceiverAccount.Id)
                .Select(g => new
                {
                    IsPendingReceiver = g.Any(i => i.Status == TransferConnectionInvitationStatus.Pending),
                    IsApprovedReceiver = g.Any(i => i.Status == TransferConnectionInvitationStatus.Approved)
                })
                .SingleOrDefaultAsync();

            var response = new GetAccountTransferRoleResponse
            {
                IsApprovedReceiver = data?.IsApprovedReceiver ?? false,
                IsPendingReceiver = data?.IsPendingReceiver ?? false
            };

            return response;
        }
    }
}