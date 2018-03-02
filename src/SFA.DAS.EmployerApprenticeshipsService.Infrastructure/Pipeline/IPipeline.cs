using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Pipeline
{
    public interface IPipeline<in T1, T2>
    {
        Task<T2> ProcessAsync(T1 obj);
    }
}
