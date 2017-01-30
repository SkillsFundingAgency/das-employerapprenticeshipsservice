using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public static class LevyDeclarationViewsObjectMother
    {
        public static List<LevyDeclarationView> Create(long accountId = 1234588, string empref = "123/abc123")
        {
            var item = new LevyDeclarationView
            {
                Id = 95875,
                AccountId = accountId,
                LevyDueYtd = 1000,
                EmpRef = empref,
                EnglishFraction = 0.90m,
                PayrollMonth = 2,
                PayrollYear = "17-18",
                SubmissionDate = new DateTime(2016, 05, 14),
                SubmissionId = 1542
            };
            
            return new List<LevyDeclarationView> {item};
        }
    }
}
