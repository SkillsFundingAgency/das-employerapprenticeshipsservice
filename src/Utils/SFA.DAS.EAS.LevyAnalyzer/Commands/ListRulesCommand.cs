using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;

namespace SFA.DAS.EAS.LevyAnalyser.Commands
{
    public class ListRulesCommand : ICommand
    {
        private readonly IRuleRepository _ruleRepository;

        public ListRulesCommand(
            IRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        public Task DoAsync(CancellationToken cancellationToken)
        {
            foreach (var rule in _ruleRepository.AvailableRules)
            {
                Console.WriteLine($"{rule.Name, 50} works-against: {rule.RequiredValidationObject}");
            }

            return Task.CompletedTask;
        }
    }
}
