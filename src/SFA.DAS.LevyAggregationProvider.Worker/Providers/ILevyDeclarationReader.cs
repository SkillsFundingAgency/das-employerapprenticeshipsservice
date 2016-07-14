using System.Threading.Tasks;
using SFA.DAS.LevyAggregationProvider.Worker.Model;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public interface ILevyDeclarationReader
    {
        Task<SourceData> GetData(int accountId);
    }
}