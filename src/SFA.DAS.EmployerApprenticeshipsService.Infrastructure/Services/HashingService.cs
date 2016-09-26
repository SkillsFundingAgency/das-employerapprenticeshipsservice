using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using HashidsNet;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class HashingService : IHashingService
    {
        public string HashValue(long id)
        {
            var hashIds = new Hashids("SFA: digital apprenticeship service",6, "46789BCDFGHJKLMNPRSTVWXY");

            return hashIds.EncodeLong(id);
        }
    }
}
