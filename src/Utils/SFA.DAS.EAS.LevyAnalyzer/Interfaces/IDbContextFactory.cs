using SFA.DAS.EAS.LevyAnalyser.Repositories;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    /// <summary>
    ///     Provides access to the db contexts required by the app.
    /// </summary>
    public interface IDbContextFactory
    {
        FinanceDbContext GetFinanceDbContext();
    }
}