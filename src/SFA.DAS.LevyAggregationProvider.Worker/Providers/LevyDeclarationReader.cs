using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.LevyAggregationProvider.Worker.Model;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyDeclarationReader : ILevyDeclarationReader
    {
        private readonly IAccountRepository _accountRepository;

        public LevyDeclarationReader(IAccountRepository accountRepository)
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            _accountRepository = accountRepository;
        }

        public async Task<SourceData> GetData(int accountId)
        {
            //TODO: Get data for the Account
            //_accountRepository

            //TODO: Convert stored data into format required by Aggregator
            return new SourceData
            {
                AccountId = accountId,
                Data = new List<SourceDataItem>()
            };
        }
    }
}