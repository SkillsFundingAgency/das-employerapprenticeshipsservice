using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.ExtensionMethods
{
    public class YearDeclarations
    {
        public string PayrollYear { get; set; }
        public LevyDeclaration[] Declarations { get; set; }
    }
}