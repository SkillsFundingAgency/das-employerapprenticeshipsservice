using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    public interface ICommand
    {
        Task DoAsync(CancellationToken cancellationToken);
    }
}