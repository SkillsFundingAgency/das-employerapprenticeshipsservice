using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DbCleanup
{
    public interface ICleanUpDatabase
    {
        Task Execute();
    }
}