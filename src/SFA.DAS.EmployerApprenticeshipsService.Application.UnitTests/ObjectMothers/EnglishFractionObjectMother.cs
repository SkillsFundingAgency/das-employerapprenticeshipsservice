using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.ObjectMothers
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