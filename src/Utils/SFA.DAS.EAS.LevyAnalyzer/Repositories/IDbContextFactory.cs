namespace SFA.DAS.EAS.LevyAnalyzer.Repositories
{
    public interface IDbContextFactory
    {
        FinanceDbContext GetFinanceDbContext();
    }
}