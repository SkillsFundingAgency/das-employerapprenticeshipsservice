using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.EmployerAccounts.Models.Commitments;

namespace SFA.DAS.EmployerAccounts.Mappings
{
    public class ApprenticeshipMappings : Profile
    {
        public ApprenticeshipMappings()
        {
            CreateMap<DraftApprenticeshipDto, Apprenticeship>()              
               .ForMember(target => target.CourseStartDate, opt => opt.MapFrom(src => src.StartDate))
               .ForMember(target => target.CourseEndDate, opt => opt.MapFrom(src => src.EndDate))
               .ForMember(target => target.ApprenticeshipStatus, opt => opt.MapFrom(src => Models.Commitments.ApprenticeshipStatus.Draft))
               .ForMember(target => target.HashedId, opt => opt.Ignore())               
               .ForMember(target => target.TrainingProvider, opt => opt.Ignore())
               .ForMember(target => target.Cohort, opt => opt.Ignore());

            CreateMap<ApprenticeshipDetailsResponse, Apprenticeship>()              
               .ForMember(target => target.CourseStartDate, opt => opt.MapFrom(src => src.StartDate))
               .ForMember(target => target.CourseEndDate, opt => opt.MapFrom(src => src.EndDate))
               .ForMember(target => target.ApprenticeshipStatus, opt => opt.MapFrom(src => Models.Commitments.ApprenticeshipStatus.Approved))
               .ForMember(target => target.HashedId, opt => opt.Ignore())               
               .ForMember(target => target.TrainingProvider, opt => opt.Ignore())
               .ForMember(target => target.Cohort, opt => opt.Ignore());

            CreateMap<CohortSummary, TrainingProvider>()
               .ForMember(target => target.Id, opt => opt.MapFrom(src => src.ProviderId))
               .ForMember(target => target.Name, opt => opt.MapFrom(src => src.ProviderName));
        }
    }
}
