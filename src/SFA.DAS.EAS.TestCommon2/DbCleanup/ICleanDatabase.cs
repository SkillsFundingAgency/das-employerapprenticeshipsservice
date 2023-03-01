using System.Threading.Tasks;

namespace SFA.DAS.EAS.TestCommon.DbCleanup
{
    public interface ICleanDatabase
    {
        Task Execute();
    }
}