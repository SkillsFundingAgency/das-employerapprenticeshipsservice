using AutoMapper;
using SFA.DAS.EmployerFinance.MarkerInterfaces;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class PublicHashedResolver : IMemberValueResolver<object, object, long, string>
    {
        private readonly IPublicHashingService _publicHashingService;

        public PublicHashedResolver(IPublicHashingService publicHashingService)
        {
            _publicHashingService = publicHashingService;
        }

        public string Resolve(object source, object destination, long sourceMember, string destinationMember, ResolutionContext context)
        {
            return _publicHashingService.HashValue(sourceMember);
        }
    }
}
