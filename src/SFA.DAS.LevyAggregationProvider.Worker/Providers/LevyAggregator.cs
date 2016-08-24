using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyAggregator
    {
        public AggregationData BuildAggregate(LevyDeclarationSourceData input)
        {
            if (input == null)
                return null;

            var aggregates = DoWork(input.Data);

            return new AggregationData
            {
                AccountId = input.AccountId,
                Data = aggregates.OrderByDescending(c => c.Year).ThenByDescending(n => n.Month).ToList()
            };
        }

        private IEnumerable<AggregationLine> DoWork(IEnumerable<LevyDeclarationSourceDataItem> source)
        {
            var output = new List<AggregationLine>();

            var balance = 0.0m;

            foreach (var item in source)
            {
                balance += item.LevyDueYtd;

                var existing = output.FirstOrDefault(x => x.LevyItemType == item.LevyItemType && x.Year == item.SubmissionDate.Year && x.Month == item.SubmissionDate.Month);

                if (existing == null)
                {
                    existing = new AggregationLine
                    {
                        Month = item.SubmissionDate.Month,
                        Year = item.SubmissionDate.Year,
                        LevyItemType = item.LevyItemType,
                        Amount = item.LevyDueYtd,
                        Balance = balance,
                        Id = item.Id,
                        Items = new List<AggregationLineItem>
                        {
                            MapFrom(item)
                        }
                    };
                    output.Add(existing);
                }
                else
                {
                    existing.Amount += item.LevyDueYtd;
                    existing.Balance = balance;
                    existing.Items.Add(MapFrom(item));
                }
            }

            return output;
        }

        private AggregationLineItem MapFrom(LevyDeclarationSourceDataItem item)
        {
            return new AggregationLineItem
            {
                Id = item.Id,
                EmpRef = item.EmpRef,
                ActivityDate = item.SubmissionDate,
                Amount = item.LevyDueYtd,
                EnglishFraction = item.EnglishFraction,
                LevyItemType = item.LevyItemType
            };
        }
    }
}