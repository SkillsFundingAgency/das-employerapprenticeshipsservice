using System.Data;
using System.Data.Common;

namespace SFA.DAS.EAS.LevyAnalyzer.Interfaces
{
    public interface IDbConnectionFactory
    {
        DbConnection GetConnection(string name);
    }
}