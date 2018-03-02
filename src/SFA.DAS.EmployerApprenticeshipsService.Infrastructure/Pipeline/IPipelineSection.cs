using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Pipeline
{
    public interface IPipelineSection<in T1, T2>
    {
        uint Priority { get; }
        Task<T2> ProcessAsync(T1 obj);
    }
}
