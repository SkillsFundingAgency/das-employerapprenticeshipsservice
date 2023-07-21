using AutoMapper;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;

namespace SFA.DAS.EmployerAccounts.Web.Mappings;

public class CohortMapping : Profile
{
    public CohortMapping()
    {
        CreateMap<Cohort, CohortViewModel>()
            .ForMember(dest => dest.HashedCohortId, opt => opt.MapFrom(src => src.HashedId));

        CreateMap<Apprenticeship, ApprenticeshipViewModel>()
            .ForMember(dest => dest.ApprenticeshipFullName, opt => opt.MapFrom(src => string.Format("{0} {1}",
                src.FirstName, src.LastName)))
            .ForMember(dest => dest.HashedApprenticeshipId, opt => opt.MapFrom(src => src.HashedId))
            .ForMember(dest => dest.CohortId, opt => opt.MapFrom(src => src.Cohort.Id))
            .ForMember(dest => dest.HashedCohortId, opt => opt.MapFrom(src => src.Cohort.HashedId))
            .ForMember(dest => dest.NumberOfDraftApprentices, opt => opt.MapFrom(src => src.Cohort.NumberOfDraftApprentices))
            .ForMember(dest => dest.CourseStartDateText, opt => opt.MapFrom(src => src.CourseStartDate.HasValue ? src.CourseStartDate.Value.ToString("dd MMMM yyyy") : ""))
            .ForMember(dest => dest.CourseEndDateText, opt => opt.MapFrom(src => src.CourseEndDate.HasValue ? src.CourseEndDate.Value.ToString("dd MMMM yyyy") : ""));                                
            
        CreateMap<TrainingProvider, TrainingProviderViewModel>();
    }
}