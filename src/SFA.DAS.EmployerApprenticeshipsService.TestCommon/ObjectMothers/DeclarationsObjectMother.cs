﻿using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;

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
                            SubmissionTime = DateTime.UtcNow.ToString(),
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
