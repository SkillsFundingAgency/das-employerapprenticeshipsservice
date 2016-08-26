using System;
using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers
{
    public static class LevyDeclarationSourceDataObjectMother
    {
        public static LevyDeclarationSourceData Create(string[] emprefs, long accountId=123453, int numberOfDeclarations = 1, int declarationsPerperiodPerPaye =1, bool randomEnlgishFraction = false)
        {
            var item = new LevyDeclarationSourceData
            {
                AccountId = accountId,
                Data = BuildItems(numberOfDeclarations,emprefs, declarationsPerperiodPerPaye, randomEnlgishFraction)
            };

            return item;
        }

        private static List<LevyDeclarationSourceDataItem> BuildItems(int numberOfDeclarations, string[] empRefs,int declarationsPerperiodPerPaye, bool randomEnglishFraction)
        {
            var randomLevyDueYtd = new Random();
            var list = new List<LevyDeclarationSourceDataItem>();
            foreach (var empref in empRefs)
            {
                for (var i = 1; i <= numberOfDeclarations; i++)
                {
                    var submissionDate = new DateTime(2016, 05, 06).AddMonths(i);

                    for (var j = 1; j <= declarationsPerperiodPerPaye; j++)
                    {
                        submissionDate = submissionDate.AddDays(j);
                        list.Add(new LevyDeclarationSourceDataItem
                        {
                            Id = i,
                            EnglishFraction = !randomEnglishFraction ? 1m : randomLevyDueYtd.Next(0, 99) / 100m,
                            PayrollDate = submissionDate,//TODO
                            SubmissionDate = submissionDate,
                            LevyItemType = 0,
                            EmpRef = empref,
                            LevyDueYtd = randomLevyDueYtd.Next(20, 1000),
                            PayrollMonth = (short)submissionDate.Month,//TODO
                            PayrollYear = submissionDate.ToString("yy"),//TODO
                            LastSubmission = j==declarationsPerperiodPerPaye ? 1:0,
                        });
                    }
                }
            }
            

            return list;
        }
    }
}