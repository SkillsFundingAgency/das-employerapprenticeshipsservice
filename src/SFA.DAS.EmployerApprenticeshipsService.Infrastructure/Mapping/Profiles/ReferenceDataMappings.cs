using AutoMapper;
using SFA.DAS.EAS.Domain.Models.ReferenceData;


namespace SFA.DAS.EAS.Infrastructure.Mapping.Profiles
{
    public class ReferenceDataMappings : Profile
    {
        public ReferenceDataMappings()
        {
            CreateMap<Charity, Charity>();
        }
    }
}
