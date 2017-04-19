using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.PAYE;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IPayeRepository
    {
        Task<PayeSchemeView> GetPayeForAccountByRef(string hashedAccountId, string payeRef);
        Task<Paye> GetPayeSchemeByRef(string payeRef);
        Task UpdatePayeSchemeName(string payeRef, string refName);
    }
}
