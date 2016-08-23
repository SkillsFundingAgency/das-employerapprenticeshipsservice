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
                        CalculatedAt = "",
                        Fractions = new List<Fraction>
                        {
                            new Fraction
                            {
                                Region = "English",
                                Value = "0.67"
                            }
                        }
                    }
                }
            };
            
            return declarations;
        }

        
    }
}