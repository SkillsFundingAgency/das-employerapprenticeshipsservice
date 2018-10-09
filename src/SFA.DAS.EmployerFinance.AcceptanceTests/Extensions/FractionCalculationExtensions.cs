using System;
using System.Collections.Generic;
using HMRC.ESFA.Levy.Api.Types;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class FractionCalculationExtensions
    {
        public static void ImportFractionCalculations(this List<FractionCalculation> fractionCalculations, Table table)
        {
            foreach (var tableRow in table.Rows)
            {
                fractionCalculations.Add(new FractionCalculation
                {
                    CalculatedAt = DateTime.Parse(tableRow["SubmissionDate"]),
                    Fractions = new List<Fraction>
                    {
                        new Fraction
                        {
                            Region = "England",
                            Value = tableRow["English_Fraction"]
                        }
                    }
                });
            }

        }
    }
}
