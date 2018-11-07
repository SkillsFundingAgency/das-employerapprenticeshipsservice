using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IPayeRepository
    {
        Task<PayeSchemeView> GetPayeForAccountByRef(string hashedAccountId, string payeRef);
        Task<Paye> GetPayeSchemeByRef(string payeRef);
        Task UpdatePayeSchemeName(string payeRef, string refName);
    }
}
