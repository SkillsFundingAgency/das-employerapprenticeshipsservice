using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.Paye;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IPayeRepository
    {
        Task<PayeSchemeView> GetPayeForAccountByRef(long accountId, string payeRef);
        Task<Paye> GetPayeSchemeByRef(string payeRef);
        Task UpdatePayeSchemeName(string payeRef, string refName);
    }
}
