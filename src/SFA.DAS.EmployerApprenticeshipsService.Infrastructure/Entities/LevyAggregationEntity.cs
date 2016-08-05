using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Entities
{
    public class LevyAggregationEntity : TableEntity
    {
        public LevyAggregationEntity()
        {
            
        }

        public LevyAggregationEntity(long accountId, int pageNumber)
            :base(accountId.ToString(), pageNumber.ToString())
        {
            
        }

        public string Data { get; set; }
    }
}