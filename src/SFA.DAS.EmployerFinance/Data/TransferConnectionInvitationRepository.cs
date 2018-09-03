﻿using SFA.DAS.EmployerFinance.Models.TransferConnections;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public class TransferConnectionInvitationRepository : ITransferConnectionInvitationRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public TransferConnectionInvitationRepository(Lazy<EmployerAccountsDbContext> db)
        {
            _db = db;
        }

        public Task Add(TransferConnectionInvitation transferConnectionInvitation)
        {
            _db.Value.TransferConnectionInvitations.Add(transferConnectionInvitation);

            return _db.Value.SaveChangesAsync();
        }

        public Task<TransferConnectionInvitation> GetTransferConnectionInvitationById(int transferConnectionInvitationId)
        {
            return _db.Value.TransferConnectionInvitations
                .Include(i => i.Changes)
                .Include(i => i.ReceiverAccount)
                .Include(i => i.SenderAccount)
                .Where(i => i.Id == transferConnectionInvitationId)
                .SingleOrDefaultAsync();
        }
    }
}