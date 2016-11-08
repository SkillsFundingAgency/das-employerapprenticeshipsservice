using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Infrastructure.Entities;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class AggregationRepository : IAggregationRepository
    {
        private readonly CloudStorageAccount _storageAccount;

        public AggregationRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }

        public async Task Update(long accountId, int pageNumber, string json)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("LevyAggregation");

            var entity = new LevyAggregationEntity(accountId, pageNumber)
            {
                Data = json
            };

            var insertOperation = TableOperation.InsertOrReplace(entity);

            await table.ExecuteAsync(insertOperation);
        }

        public async Task<AggregationData> GetByAccountId(long accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AggregationData>> GetByAccountIds(List<long> accountIds)
        {
            throw new NotImplementedException();


        }
    }
}