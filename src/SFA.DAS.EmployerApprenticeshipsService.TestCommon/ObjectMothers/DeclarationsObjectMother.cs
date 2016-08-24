using System;
using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers
{
    public static class DeclarationsObjectMother
    {
        public static LevyDeclarations Create(string empRef)
        {
            var declarations = new LevyDeclarations
            {
                Declarations =
                    new List<Declaration>
                    {
                        new Declaration
                        {
                            PayrollPeriod = new PayrollPeriod {Month = 10, Year = "2016"},
                            SubmissionTime = "",
                            DateCeased = DateTime.Now
                        }
                    },
                EmpRef = empRef
            };

            return declarations;
        }
    }
}
