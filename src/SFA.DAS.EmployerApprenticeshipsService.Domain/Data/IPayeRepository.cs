using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IPayeRepository
    {
        Task<PayeSchemeView> GetPayeByRef(string payeRef);
    }
}
