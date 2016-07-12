using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers
{
    public static class EnglishFractionObjectMother
    {
        public static EnglishFractionDeclarations Create(string empRef)
        {
            var declarations = new EnglishFractionDeclarations
            {
                Empref = empRef,
                FractionCalculations = new List<FractionCalculation>
                {
                    new FractionCalculation
                    {
                        calculatedAt = "",
                        fractions = new List<Fraction>
                        {
                            new Fraction
                            {
                                region = "English",
                                value = "0.67"
                            }
                        }
                    }
                }
            };
            
            return declarations;
        }

        
    }
}