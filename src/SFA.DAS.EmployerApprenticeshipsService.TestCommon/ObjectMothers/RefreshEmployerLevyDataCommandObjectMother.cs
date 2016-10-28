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
                            Date = DateTime.UtcNow
                        },
                        new DasDeclaration
                        {
                            Id = "2",
                            LevyDueYtd = 70,
                            Date = DateTime.UtcNow.AddMonths(1)
                        },
                        new DasDeclaration
                        {
                            Id = "3",
                            NoPaymentForPeriod = true,
                            Date = DateTime.UtcNow.AddMonths(2)
                        },
                        new DasDeclaration
                        {
                            Id = "4",
                            LevyDueYtd = 80,
                            Date = DateTime.UtcNow.AddMonths(3)
                        }
                    }
                },
                Fractions = new DasEnglishFractions
                {
                    Fractions = new List<DasEnglishFraction>
                    {
                        new DasEnglishFraction
                        {
                            Amount = 0.89m,
                            Id = "1",
                            DateCalculated = DateTime.UtcNow
                        },
                        new DasEnglishFraction
                        {
                            Amount = 0.89m,
                            Id = "1",
                            DateCalculated = DateTime.UtcNow
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
