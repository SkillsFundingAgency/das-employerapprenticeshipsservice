using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public static class LevyDeclarationSourceDataObjectMother
    {
        public static LevyDeclarationSourceData Create(List<Emprefs> emprefs, long accountId = 123453, int numberOfDeclarations = 1, int declarationsPerperiodPerPaye = 1, bool randomEnlgishFraction = false, bool addTopUp = false, DateTime? submissionStartDate = null,bool multipleAccountIds = false)
        {
            if (!submissionStartDate.HasValue)
            {
                submissionStartDate = new DateTime(2016, 05, 06);
            }

            var item = new LevyDeclarationSourceData
            {
                AccountId = accountId,
                Data = BuildItems(numberOfDeclarations, emprefs, declarationsPerperiodPerPaye, randomEnlgishFraction, addTopUp, submissionStartDate.Value, accountId, multipleAccountIds)
            };

            return item;
        }

        public static LevyDeclarationSourceData CreateStatic(int numberOfItems = 1)
        {
            var returnValue =  new LevyDeclarationSourceData
            {
                AccountId = 1,
                Data = NewLevyDeclarationSourceDataItem(numberOfItems)
            
            };

            return returnValue;
        }

        private static List<LevyDeclarationSourceDataItem> NewLevyDeclarationSourceDataItem(int numberOfItems)
        {

            var items = new List<LevyDeclarationSourceDataItem>();

            for (var i = 0; i < numberOfItems; i++)
            {
                items.Add(new LevyDeclarationSourceDataItem
                {
                    AccountId = 1,
                    EmpRef = "123/ABC",
                    EmprefAddedDate = new DateTime(2016, 01, 01),
                    EnglishFraction = 0.87544m,
                    Id = 1,
                    LastSubmission = 1,
                    LevyDueYtd = 2011.145m,
                    PayrollDate = new DateTime(2016, 01, 01),
                    PayrollMonth = 1,
                    PayrollYear = "15-16",
                    SubmissionDate = new DateTime(2016, 01, 16),
                    TopUp = 201.1545m

                });
            }

            return items;
        }

        private static List<LevyDeclarationSourceDataItem> BuildItems(int numberOfDeclarations, List<Emprefs> emprefs, int declarationsPerperiodPerPaye, bool randomEnglishFraction, bool addTopup, DateTime submissionStartDate, long accountId, bool multipleAccountIds)
        {
            var randomLevyDueYtd = new Random();
            var list = new List<LevyDeclarationSourceDataItem>();
            foreach (var empref in emprefs)
            {
                bool idUpdated = false;
                for (var i = 1; i <= numberOfDeclarations; i++)
                {
                    var submissionDate = submissionStartDate.AddMonths(i);
                    
                    for (var j = 1; j <= declarationsPerperiodPerPaye; j++)
                    {
                        submissionDate = submissionDate.AddDays(j);
                        var levyDueYtd = randomLevyDueYtd.Next(20, 1000);


                        //var newAccountId = 
                        if (empref.DeclarationsForScheme!= 0 && i > empref.DeclarationsForScheme && !idUpdated)
                        {
                            accountId = accountId + 1;
                            idUpdated = true;
                        }

                        list.Add(new LevyDeclarationSourceDataItem
                        {
                            Id = i,
                            EnglishFraction = !randomEnglishFraction ? 1m : randomLevyDueYtd.Next(0, 99) / 100m,
                            PayrollDate = submissionDate,//TODO
                            SubmissionDate = submissionDate,
                            LevyItemType = 0,
                            EmpRef = empref.Empref,
                            EmprefAddedDate = empref.AddedDate,
                            EmprefRemovedDate = empref.RemovedDate,
                            LevyDueYtd = levyDueYtd,
                            PayrollMonth = (short)submissionDate.Month,//TODO
                            PayrollYear = submissionDate.ToString("yy"),//TODO
                            LastSubmission = j == declarationsPerperiodPerPaye ? 1 : 0,
                            TopUp = addTopup ? levyDueYtd * 0.1m : 0m,
                            AccountId =  accountId
                        });
                    }
                }
            }


            return list;
        }

        public class Emprefs
        {
            public string Empref { get; set; }
            public DateTime AddedDate { get; set; }
            public DateTime? RemovedDate { get; set; }
            public int DeclarationsForScheme { get; set; }
        }
    }
}