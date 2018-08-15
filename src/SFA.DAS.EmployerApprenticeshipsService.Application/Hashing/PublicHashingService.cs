using SFA.DAS.EAS.Infrastructure.Interfaces;

namespace SFA.DAS.EAS.Application.Hashing
{
    public class IalePublicHashingService : HashingService.HashingService, IPublicHashingService, IALEPublicHashingService
    {
        public IalePublicHashingService(string allowedCharacters, string hashstring) : base(allowedCharacters, hashstring)
        {
        }
    }
}