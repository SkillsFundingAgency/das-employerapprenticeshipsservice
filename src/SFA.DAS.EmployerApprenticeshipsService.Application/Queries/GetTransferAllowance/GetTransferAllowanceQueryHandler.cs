﻿using MediatR;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQueryHandler : IAsyncRequestHandler<GetTransferAllowanceQuery, GetTransferAllowanceResponse>
    {
        private readonly EmployerFinancialDbContext _db;
        private readonly LevyDeclarationProviderConfiguration _configuration;

        public GetTransferAllowanceQueryHandler(EmployerFinancialDbContext db, LevyDeclarationProviderConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<GetTransferAllowanceResponse> Handle(GetTransferAllowanceQuery message)
        {
            var transferAllowance = await _db.GetTransferAllowance(message.AccountId.Value, _configuration.TransferAllowancePercentage);

            transferAllowance = transferAllowance < 0 ? 0 : transferAllowance;

            return new GetTransferAllowanceResponse
            {
                TransferAllowance = transferAllowance 
            };
        }
    }
}