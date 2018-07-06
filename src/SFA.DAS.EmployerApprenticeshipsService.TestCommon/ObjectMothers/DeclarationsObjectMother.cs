using System;
using System.Collections.Generic;
using HMRC.ESFA.Levy.Api.Types;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
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
                            SubmissionTime = DateTime.UtcNow,
                            DateCeased = DateTime.UtcNow,
                            NoPaymentForPeriod = true
                        }
                    },
                EmpRef = empRef
            };

            return declarations;
        }
    }
}
