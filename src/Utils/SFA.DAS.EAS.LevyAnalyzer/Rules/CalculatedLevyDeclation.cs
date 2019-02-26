using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.Rules
{
    public class CalculatedLevyDeclation
    {
        public CalculatedLevyDeclation(LevyDeclaration current, LevyDeclaration previous)
        {
            CurrentDeclaration = current;

            if (CurrentDeclaration.PayrollMonth.Value != 1 && current.PayrollYear == previous?.PayrollYear)
            {
                PreviousDeclaration = previous;
            }
        }

        public decimal CalculatedLevyAmountForMonth => (CurrentDeclaration.LevyDueYTD ?? 0) - (PreviousDeclaration?.LevyDueYTD ?? 0);
        public LevyDeclaration CurrentDeclaration { get; }
        public LevyDeclaration PreviousDeclaration { get; }
    }
}