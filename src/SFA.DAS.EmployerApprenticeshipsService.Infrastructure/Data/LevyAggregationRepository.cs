using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Entities;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class LevyAggregationRepository : IAggregationRepository
    {
        private readonly CloudStorageAccount _storageAccount;

        //public LevyAggregationRepository() 
        //    : this(CloudConfigurationManager.GetSetting("StorageConnectionString"))
        //{
            
        //}

        public LevyAggregationRepository(string storageConnectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        }

        public async Task Update(int accountId, int pageNumber, string json)
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
    }
}