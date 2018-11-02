using System.Threading.Tasks;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    /// <summary>
    ///     Represents a service that can save command results.
    /// </summary>
    public interface IResultSaver
    {
        Task SaveAsync<TResult>(TResult results);
    }
}
