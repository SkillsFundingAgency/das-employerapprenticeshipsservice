﻿using MediatR;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;

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

            return new GetTransferAllowanceResponse
            {
                TransferAllowance = transferAllowance
            };
        }
    }
}