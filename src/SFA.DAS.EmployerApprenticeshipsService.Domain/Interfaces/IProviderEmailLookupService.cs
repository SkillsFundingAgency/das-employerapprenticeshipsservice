using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IProviderEmailLookupService
    {
        Task<List<string>> GetEmailsAsync(long providerId, string lastUpdateEmail);
    }
}