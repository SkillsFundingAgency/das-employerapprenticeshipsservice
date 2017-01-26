using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface ILegalEntityRepository
    {
        Task<LegalEntityView> GetLegalEntityById(long id);
    }
}
