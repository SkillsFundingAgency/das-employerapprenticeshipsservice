using AutoMapper;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class HashedResolver : IMemberValueResolver<object, object, long, string>
    {
        private readonly IHashingService _hashingService;

        public HashedResolver(IHashingService hashingService)
        {
            _hashingService = hashingService;
        }

        public string Resolve(object source, object destination, long sourceMember, string destinationMember, ResolutionContext context)
        {
            return _hashingService.HashValue(sourceMember);
        }
    }
}
