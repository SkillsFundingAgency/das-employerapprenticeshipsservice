using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    /// <summary>
    ///     A factory for creating <see cref="DeclarationSummary"/> instances.
    /// </summary>
    public interface IDeclarationSummaryFactory
    {
        DeclarationSummary Create(Account account, int index);
    }
}