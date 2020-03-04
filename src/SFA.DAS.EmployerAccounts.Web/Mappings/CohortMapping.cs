using AutoMapper;
using SFA.DAS.EmployerAccounts.Models.Commitments;
using SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Mappings
{
    public class CohortMapping : Profile
    {
        public CohortMapping()
        {
            CreateMap<CohortV2, CohortViewModel>()
               .ForMember(dest => dest.CohortId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.HashedCohortReference, opt => opt.MapFrom(src => src.HashedId));

            CreateMap<GetSingleCohortResponse, CohortViewModel>()
                .ForMember(dest => dest.CohortId, opt => opt.Ignore())
                .ForMember(dest => dest.NumberOfDraftApprentices, opt => opt.Ignore())
                .ForMember(dest => dest.CohortStatus, opt => opt.Ignore())
                .ForMember(dest => dest.HashedCohortReference, opt => opt.Ignore())
                .ForMember(dest => dest.Apprenticeships, opt => opt.Ignore());                

            CreateMap<Apprenticeship, ApprenticeshipViewModel>()
                .ForMember(dest => dest.ApprenticeshipFullName, opt => opt.MapFrom(src => string.Format("{0} {1}",
                                       src.FirstName, src.LastName)))                
                .ForMember(dest => dest.HashedApprenticeshipId, opt => opt.MapFrom(src => src.HashedId))
                .ForMember(dest => dest.CohortId, opt => opt.MapFrom(src => src.Cohort.Id))
                .ForMember(dest => dest.HashedCohortId, opt => opt.MapFrom(src => src.Cohort.HashedId))
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                
                .ForMember(dest => dest.LastName, opt => opt.Ignore());

            CreateMap<TrainingProvider, TrainingProviderViewModel>();
                
        }
    }
}