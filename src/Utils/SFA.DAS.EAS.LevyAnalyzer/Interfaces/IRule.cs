using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyzer.Models;
using SFA.DAS.EAS.LevyAnalyzer.Rules;
using SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyzer.Interfaces
{
    public interface IRule
    {
        string Name { get; }
        void Validate(Account account, RuleEvaluationResult validationResult);
    }
}
