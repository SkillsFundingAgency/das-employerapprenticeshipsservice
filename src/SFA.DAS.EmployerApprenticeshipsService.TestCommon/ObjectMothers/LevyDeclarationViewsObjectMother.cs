using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers
{
    public static class LevyDeclarationViewsObjectMother
    {
        public static List<LevyDeclarationView> Create(long accountId = 1234588)
        {
            var item = new LevyDeclarationView
            {
                Id = 95875,
                AccountId = accountId,
                LevyDueYtd = 1000,
                EmpRef = "123/abc123",
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
