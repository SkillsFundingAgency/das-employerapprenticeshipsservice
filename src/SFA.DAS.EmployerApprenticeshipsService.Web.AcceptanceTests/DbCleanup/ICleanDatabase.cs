using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DbCleanup
{
    public interface ICleanDatabase
    {
        Task Execute();
    }
}