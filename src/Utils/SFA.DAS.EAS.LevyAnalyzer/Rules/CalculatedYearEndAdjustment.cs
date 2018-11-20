using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.Rules
{
    public class CalculatedYearEndAdjustment
    {
        public CalculatedYearEndAdjustment(LevyDeclaration current, LevyDeclaration previous)
        {
            CurrentDeclaration = current;

            if (CurrentDeclaration.PayrollMonth.Value != 1)
            {
                PreviousDeclaration = previous;
            }
        }

        public decimal CalculatedAdjustment => (CurrentDeclaration.LevyDueYTD ?? 0) - (PreviousDeclaration?.LevyDueYTD ?? 0);
        public LevyDeclaration CurrentDeclaration { get; }
        public LevyDeclaration PreviousDeclaration { get; }
    }
}