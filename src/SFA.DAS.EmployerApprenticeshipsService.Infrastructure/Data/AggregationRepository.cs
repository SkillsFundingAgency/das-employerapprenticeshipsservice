﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Entities;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
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
            var tableClient = _storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("LevyAggregation");

            var fetchOperation = TableOperation.Retrieve<LevyAggregationEntity>(accountId.ToString(), 1.ToString());
            var result = await table.ExecuteAsync(fetchOperation);
            var row = (LevyAggregationEntity)result.Result;

            if (row == null)
                return new AggregationData
                {
                    AccountId = accountId,
                    Data = new List<AggregationLine>()
                };

            return JsonConvert.DeserializeObject<AggregationData>(row.Data);
        }

        public async Task<List<AggregationData>> GetByAccountIds(List<long> accountIds)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("LevyAggregation");

            var response = new List<AggregationData>();
            
            foreach (var accountId in accountIds)
            {
                var fetchOperation = TableOperation.Retrieve<LevyAggregationEntity>(accountId.ToString(), 1.ToString());
                var result = await table.ExecuteAsync(fetchOperation);
                var row = (LevyAggregationEntity) result.Result;

                if (row == null)
                {
                    response.Add(new AggregationData
                    {
                        AccountId = accountId,
                        Data = new List<AggregationLine>()
                    });
                }
                else
                {
                    response.Add(JsonConvert.DeserializeObject<AggregationData>(row.Data));
                }
            }

            return response;


        }
    }
}