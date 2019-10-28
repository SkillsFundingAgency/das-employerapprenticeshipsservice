using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IReservationsApiClient
    {
        Task<string> Get(long accountId);
    }
}
