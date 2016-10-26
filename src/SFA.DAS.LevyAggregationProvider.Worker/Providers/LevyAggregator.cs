using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
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

            var aggregates = AggregateData(input.Data, input.AccountId);

            return new AggregationData
            {
                AccountId = input.AccountId,
                Data = aggregates.OrderByDescending(c => c.Year).ThenByDescending(n => n.Month).ToList()
            };
        }

        private IEnumerable<AggregationLine> AggregateData(IEnumerable<LevyDeclarationSourceDataItem> source, long accountId)
        {
            var output = new List<AggregationLine>();
            

            foreach (var declarationsForMonth in GetDeclarationsByPayrollYearAndPayrollMonth(source))
            {
                var previousAmount = new Dictionary<string, decimal>();
                var aggregationLine = BuildAggregationLine(declarationsForMonth, output);

                AddPreviousAmountForPayeToCollection(previousAmount, output);

                foreach (var declarationsByEmpref in GetDeclarationsByEmpref(declarationsForMonth))
                {
                    foreach (LevyDeclarationSourceDataItem levyDeclarationSourceDataItem in declarationsByEmpref.Data)
                    {
                        aggregationLine.Items.Add(MapFrom(levyDeclarationSourceDataItem, previousAmount));
                        if (levyDeclarationSourceDataItem.TopUp != 0)
                        {
                            aggregationLine.Items.Add(MapTopUp(levyDeclarationSourceDataItem));
                        }
                    }
                }

                var amount = CalculateAmountWithEnglishFraction(aggregationLine.Items, accountId);
                aggregationLine.Amount = amount;
                
                if (string.IsNullOrEmpty(FindAggregationLine(declarationsForMonth, output)))
                {
                    aggregationLine.Balance = CalculateCurrentBalance(output, amount);
                    output.Add(aggregationLine);
                }
                else
                {
                    aggregationLine.Balance = output.Sum(c => c.Amount);
                }
                
            }
            
            return BuildAggregationOutputForAccount(accountId, output);
        }

        private List<AggregationLine> BuildAggregationOutputForAccount(long accountId, List<AggregationLine> output)
        {
            var aggregationOutputForAccount = new List<AggregationLine>();
            foreach (var item in output)
            {
                var newLine = new AggregationLine {Items = new List<AggregationLineItem>()};
                foreach (var lineItem in item.Items)
                {
                    if (lineItem.AccountId == accountId)
                    {
                        newLine.Items.Add(lineItem);
                    }
                }

                if (newLine.Items.Any())
                {
                    newLine.Id = item.Id;
                    newLine.Amount = item.Amount;
                    newLine.Balance = item.Balance;
                    newLine.LevyItemType = item.LevyItemType;
                    newLine.Month = item.Month;
                    newLine.Year = item.Year;
                    aggregationOutputForAccount.Add(newLine);
                }
            }

            return aggregationOutputForAccount;
        }

        private static AggregationLine BuildAggregationLine(dynamic declarationsForMonth, List<AggregationLine> output)
        {
            var aggregationId = FindAggregationLine(declarationsForMonth,output);
            if (!string.IsNullOrEmpty(aggregationId))
            {
                return output.Find(c => c.Id.Equals(aggregationId));
            }
                

            return new AggregationLine
            {
                Id = Guid.NewGuid().ToString(),
                LevyItemType = LevyItemType.Declaration,
                Items = new List<AggregationLineItem>(),
                Year = declarationsForMonth.PayrollYear,
                Month = declarationsForMonth.PayrollMonth
            };
        }

        private static string FindAggregationLine(dynamic declarationsForMonth, List<AggregationLine> output)
        {
            foreach (LevyDeclarationSourceDataItem declaration in declarationsForMonth.Data)
            {
                if (declaration.EmprefAddedDate >= declaration.SubmissionDate)
                {
                    foreach (var item in output)
                    {
                        if (item.Items.FirstOrDefault(c => c.EmpRef.Equals(declaration.EmpRef)) != null)
                        {
                            {
                                return item.Id;
                                
                            }
                        }
                    }
                }
            }
            return null;
        }

        private decimal CalculateCurrentBalance(List<AggregationLine> output, decimal amount)
        {
            return output.Sum(c => c.Amount) + amount;
        }

        private decimal CalculateAmountWithEnglishFraction(List<AggregationLineItem> items, long accountId)
        {
            var totalAmount = items.GroupBy(c => new { c.EmpRef, c.AccountId }, (empref, group) => new
            {
                empref.EmpRef,
                Data = group.ToList()
            }).Sum(item =>
            {
                return item.Data.Where(c => c.IsLastSubmission && c.AccountId == accountId).Sum(c => c.CalculatedAmount);
            });

            return totalAmount;
        }

        private dynamic GetDeclarationsByEmpref(dynamic groupedItem)
        {
            return ((List<LevyDeclarationSourceDataItem>)groupedItem.Data).ToList().GroupBy(c => new { c.EmpRef }, (empref, group) => new
            {
                empref.EmpRef,
                Data = group.ToList()
            });
        }

        private dynamic GetDeclarationsByPayrollYearAndPayrollMonth(IEnumerable<LevyDeclarationSourceDataItem> source)
        {

            return source.GroupBy(c => new { c.PayrollDate.Value.Month, c.PayrollDate.Value.Year },
                (payroll, group) => new
                {
                    PayrollYear = payroll.Year,
                    PayrollMonth = payroll.Month,
                    Data = group.ToList()
                }).OrderBy(c => new PayrollDate { PayrollMonth = c.PayrollMonth, PayrollYear = c.PayrollYear }, new PayrollDateComparer());
        }

        private AggregationLineItem MapFrom(LevyDeclarationSourceDataItem item, Dictionary<string, decimal> previousAmount)
        {
            var calculatedAmount = Math.Round(item.LevyDueYtd * item.EnglishFraction, 2) - (previousAmount.ContainsKey(item.EmpRef) ? previousAmount[item.EmpRef] : 0m);
            return new AggregationLineItem
            {
                Id = item.Id,
                EmpRef = item.EmpRef,
                ActivityDate = item.SubmissionDate,
                LevyDueYtd = item.LevyDueYtd,
                Amount = Math.Round(item.LevyDueYtd * item.EnglishFraction,2),
                EnglishFraction = item.EnglishFraction,
                CalculatedAmount = calculatedAmount,
                LevyItemType = LevyItemType.Declaration,
                IsLastSubmission = item.LastSubmission == 1,
                AccountId = item.AccountId
            };
        }

        private AggregationLineItem MapTopUp(LevyDeclarationSourceDataItem item)
        {
            return new AggregationLineItem
            {
                Id = item.Id,
                EmpRef = item.EmpRef,
                ActivityDate = item.SubmissionDate,
                LevyDueYtd = 0,
                Amount = item.TopUp * item.EnglishFraction,
                CalculatedAmount = item.TopUp * item.EnglishFraction,
                EnglishFraction = item.EnglishFraction,
                LevyItemType = LevyItemType.TopUp,
                IsLastSubmission = true,
                AccountId = item.AccountId
            };
        }

        private static void AddPreviousAmountForPayeToCollection(Dictionary<string, decimal> previousAmount, List<AggregationLine> addedDeclarations)
        {
            //find all distinct emprefs

            var emprefs = new List<string>();
            foreach (var declaration in addedDeclarations)
            {
                emprefs.AddRange(declaration.Items.Select(c => c.EmpRef).Distinct());
            }


            //find the latest value for each one and add to store

            foreach (var declaration in addedDeclarations.OrderByDescending(c => new PayrollDate { PayrollMonth = c.Month, PayrollYear = c.Year }, new PayrollDateComparer()))
            {
                foreach (var aggregationLineItem in declaration.Items.Where(c => c.IsLastSubmission).OrderByDescending(c=>c.ActivityDate))
                {
                    foreach (var empref in emprefs)
                    {
                        if (!empref.Equals(aggregationLineItem.EmpRef))
                        {
                            continue;
                        }
                        if (previousAmount.ContainsKey(empref))
                        {
                            continue;
                        }
                        previousAmount.Add(empref, aggregationLineItem.Amount);
                        break;
                    }
                }
            }

            //Check to see if we have a value for all emprefs
            foreach (var empref in emprefs)
            {
                if (!previousAmount.ContainsKey(empref))
                {
                    previousAmount.Add(empref, 0m);
                }
            }

        }

        private class PayrollDate
        {
            public int PayrollMonth { get; set; }

            public int PayrollYear { get; set; }
        }

        private class PayrollDateComparer : IComparer<PayrollDate>
        {
            public int Compare(PayrollDate x, PayrollDate y)
            {
                var firstDate = new DateTime(2000 + x.PayrollYear, x.PayrollMonth, 1);
                var secondDate = new DateTime(2000 + y.PayrollYear, y.PayrollMonth, 1);

                return firstDate.CompareTo(secondDate);
            }
        }
    }
}