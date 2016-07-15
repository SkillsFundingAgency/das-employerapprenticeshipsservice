using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.LevyAggregationProvider.Worker.Model;

namespace SFA.DAS.LevyAggregationProvider.Worker.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationQueryHandler : IAsyncRequestHandler<GetLevyDeclarationRequest, GetLevyDeclarationResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public GetLevyDeclarationQueryHandler(IAccountRepository accountRepository)
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            _accountRepository = accountRepository;
        }

        public async Task<GetLevyDeclarationResponse> Handle(GetLevyDeclarationRequest message)
        {
            //TODO: Get data for the Account
            //_accountRepository

            //TODO: Convert stored data into format required by Aggregator
            return new GetLevyDeclarationResponse
            { 
                Data = new SourceData
                {
                    AccountId = message.AccountId,
                    Data = new List<SourceDataItem>()
                }
            };
        }
    }
}