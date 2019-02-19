using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    /// <summary>
    ///     Represents a service that can save command results.
    /// </summary>
    public interface ISummarySaver
    {
        Task SaveAsync(AllAccountValidationResult results);
    }
}
