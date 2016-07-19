using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.LevyAggregationProvider.Worker.Model;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyAggregator
    {
        public AggregationData BuildAggregate(SourceData input)
        {
            if (input == null)
                return null;

            var aggregates = DoWork(input.Data);

            return new AggregationData
            {
                AccountId = input.AccountId,
                Data = aggregates
            };
        }

        private List<AggregationLine> DoWork(List<SourceDataItem> source)
        {
            var output = new List<AggregationLine>();

            var balance = 0.0m;

            foreach (var item in source)
            {
                balance += item.Amount;

                var existing = output.FirstOrDefault(x => x.LevyItemType == item.LevyItemType && x.Year == item.ActivityDate.Year && x.Month == item.ActivityDate.Month);

                if (existing == null)
                {
                    existing = new AggregationLine
                    {
                        Month = item.ActivityDate.Month,
                        Year = item.ActivityDate.Year,
                        LevyItemType = item.LevyItemType,
                        Amount = item.Amount,
                        Balance = balance,
                        Items = new List<AggregationLineItem>
                        {
                            MapFrom(item)
                        }
                    };
                    output.Add(existing);
                }
                else
                {
                    existing.Amount += item.Amount;
                    existing.Balance = balance;
                    existing.Items.Add(MapFrom(item));
                }
            }

            return output;
        }

        private AggregationLineItem MapFrom(SourceDataItem item)
        {
            return new AggregationLineItem
            {
                Id = item.Id,
                EmpRef = item.EmpRef,
                ActivityDate = item.ActivityDate,
                Amount = item.Amount,
                EnglishFraction = item.EnglishFraction,
                LevyItemType = item.LevyItemType
            };
        }
    }
}