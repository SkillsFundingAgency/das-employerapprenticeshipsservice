using System;
using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.ObjectMothers
{
    public static class DeclarationsObjectMother
    {
        public static Declarations Create(string empRef)
        {

            var declarations = new Declarations
            {
                declarations =
                    new List<Declaration>
                    {
                        new Declaration
                        {
                            amount = 10,
                            payrollMonth = new PayrollMonth {month = 10, year = 2016},
                            submissionDate = "",
                            submissionType = ""
                        }
                    },
                empref = empRef,
                schemeCessationDate = DateTime.Now.ToString("yyyy-MM-dd")
            };


            return declarations;
        }
    }
}
