using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public static class RefreshEmployerLevyDataCommandObjectMother
    {
        public static RefreshEmployerLevyDataCommand Create(string empRef,long accountId = 1)
        {

            var refreshEmployerLevyDataCommand = new RefreshEmployerLevyDataCommand
            {
                AccountId = accountId,
                EmployerLevyData = new List<EmployerLevyData> {
                    new EmployerLevyData
                {
                EmpRef = empRef,
                Declarations = new DasDeclarations
                {
                    Declarations = new List<DasDeclaration>
                    {
                        new DasDeclaration
                        {
                            Id = "1",
                            LevyDueYtd = 10,
                            SubmissionDate = DateTime.UtcNow
                        },
                        new DasDeclaration
                        {
                            Id = "2",
                            LevyDueYtd = 70,
                            SubmissionDate = DateTime.UtcNow.AddMonths(1)
                        },
                        new DasDeclaration
                        {
                            Id = "3",
                            NoPaymentForPeriod = true,
                            SubmissionDate = DateTime.UtcNow.AddMonths(2)
                        },
                        new DasDeclaration
                        {
                            Id = "4",
                            LevyDueYtd = 80,
                            SubmissionDate = DateTime.UtcNow.AddMonths(3)
                        }
                    }
                }
                }
               }

            };


            return refreshEmployerLevyDataCommand;
        }
    }
}
