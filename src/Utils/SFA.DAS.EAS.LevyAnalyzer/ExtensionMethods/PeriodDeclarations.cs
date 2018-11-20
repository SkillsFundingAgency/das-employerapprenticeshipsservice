using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.ExtensionMethods
{
    public class PeriodDeclarations
    {
        public PayrollPeriod PayrollPeriod { get; set; }
        public LevyDeclaration[] Declarations { get; set; }
    }
}