using System.Data.Common;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    public interface IDbConnectionFactory
    {
        DbConnection GetConnection(string name);
    }
}