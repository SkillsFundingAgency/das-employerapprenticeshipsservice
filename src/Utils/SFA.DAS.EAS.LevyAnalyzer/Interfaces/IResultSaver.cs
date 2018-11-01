using System.Threading.Tasks;

namespace SFA.DAS.EAS.LevyAnalyzer.Interfaces
{
    public interface IResultSaver
    {
        Task SaveAsync<TResult>(TResult results);
    }
}
