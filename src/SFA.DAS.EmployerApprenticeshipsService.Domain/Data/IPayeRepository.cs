using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IPayeRepository
    {
        Task<PayeSchemeView> GetPayeForAccountByRef(string hashedAccountId, string payeRef);
    }
}
