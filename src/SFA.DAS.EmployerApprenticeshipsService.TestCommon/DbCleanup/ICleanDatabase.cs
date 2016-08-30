using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.TestCommon.DbCleanup
{
    public interface ICleanDatabase
    {
        Task Execute();
    }
}