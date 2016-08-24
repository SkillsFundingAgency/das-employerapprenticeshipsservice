using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers
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
