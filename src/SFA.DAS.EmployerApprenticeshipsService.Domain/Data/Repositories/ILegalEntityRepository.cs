using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface ILegalEntityRepository
    {
        Task<LegalEntityView> GetLegalEntityById(long accountId, long id);
    }
}
