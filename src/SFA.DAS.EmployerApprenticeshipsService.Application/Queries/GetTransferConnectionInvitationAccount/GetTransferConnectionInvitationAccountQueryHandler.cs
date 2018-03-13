using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount
{
    public class GetTransferConnectionInvitationAccountQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationAccountQuery, GetTransferConnectionInvitationAccountResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetTransferConnectionInvitationAccountQueryHandler(EmployerAccountDbContext db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetTransferConnectionInvitationAccountResponse> Handle(GetTransferConnectionInvitationAccountQuery message)
        {
            var receiverAccount = await _db.Accounts
                .ProjectTo<AccountDto>(_configurationProvider)
                .SingleOrDefaultAsync(a => a.PublicHashedId == message.ReceiverAccountPublicHashedId);

            if (receiverAccount == null)
            {
                throw new ValidationException<GetTransferConnectionInvitationAccountQuery>(q => q.ReceiverAccountPublicHashedId, "You must enter a valid account ID");
            }

            var isReceiverASender = await _db.TransferConnectionInvitations.AnyAsync(tci =>
                tci.SenderAccountId == receiverAccount.Id && tci.Status != TransferConnectionInvitationStatus.Rejected);

            if (isReceiverASender)
            {
                throw new ValidationException<GetTransferConnectionInvitationAccountQuery>(q => q.ReceiverAccountPublicHashedId, "You can’t connect with this employer because they already have pending or accepted connection requests");
            }

            var anyPendingtransferConnectionInvitations = await _db.TransferConnectionInvitations
                .Where(i => 
                    i.SenderAccount.Id == message.AccountId.Value &&
                    i.ReceiverAccount.Id == receiverAccount.Id &&
                    i.Status == TransferConnectionInvitationStatus.Pending)
                .AnyAsync();

            if (anyPendingtransferConnectionInvitations)
            {
                throw new ValidationException<GetTransferConnectionInvitationAccountQuery>(q => q.ReceiverAccountPublicHashedId, "You've already sent a connection request to this employer");
            }

            var anyApprovedtransferConnectionInvitations = await _db.TransferConnectionInvitations
                .Where(i => (
                    i.SenderAccount.Id == message.AccountId.Value && i.ReceiverAccount.Id == receiverAccount.Id ||
                    i.SenderAccount.Id == receiverAccount.Id && i.ReceiverAccount.Id == message.AccountId.Value) &&
                    i.Status == TransferConnectionInvitationStatus.Approved)
                .AnyAsync();

            if (anyApprovedtransferConnectionInvitations)
            {
                throw new ValidationException<GetTransferConnectionInvitationAccountQuery>(q => q.ReceiverAccountPublicHashedId, "You're already connected with this employer");
            }

            return new GetTransferConnectionInvitationAccountResponse
            {
                ReceiverAccount = receiverAccount
            };
        }
    }
}