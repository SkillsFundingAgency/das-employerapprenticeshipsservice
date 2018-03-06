using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionRoles
{
    public class GetTransferConnectionRolesQueryHandler : IAsyncRequestHandler<GetTransferConnectionRolesQuery, GetTransferConnectionRolesResponse>
    {
        private readonly EmployerAccountDbContext _db;

        public GetTransferConnectionRolesQueryHandler(EmployerAccountDbContext db)
        {
            _db = db;
        }

        public async Task<GetTransferConnectionRolesResponse> Handle(GetTransferConnectionRolesQuery message)
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

            var response = new GetTransferConnectionRolesResponse
            {
                IsApprovedReceiver = data?.IsApprovedReceiver ?? false,
                IsPendingReceiver = data?.IsPendingReceiver ?? false
            };

            return response;
        }
    }
}