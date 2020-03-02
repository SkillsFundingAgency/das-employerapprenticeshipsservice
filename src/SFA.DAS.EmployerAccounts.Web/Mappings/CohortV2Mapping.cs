using AutoMapper;
using SFA.DAS.EmployerAccounts.Models.Commitments;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Mappings
{
    public class CohortV2Mapping : Profile
    {
        public CohortV2Mapping()
        {
            CreateMap<CohortV2, CohortV2ViewModel>()
               .ForMember(dest => dest.CohortId, opt => opt.MapFrom(src => src.CohortId))
               .ForMember(dest => dest.CohortsCount, opt => opt.MapFrom(src => src.CohortsCount))
               .ForMember(dest => dest.CohortStatus, opt => opt.MapFrom(src => src.CohortStatus))
               .ForMember(dest => dest.NumberOfDraftApprentices, opt => opt.MapFrom(src => src.NumberOfDraftApprentices))
               .ForMember(dest => dest.HashedCohortReference, opt => opt.MapFrom(src => src.HashedCohortReference))
               .ForMember(dest => dest.HashedDraftApprenticeshipId, opt => opt.MapFrom(src => src.HashedDraftApprenticeshipId))               
               .ForMember(dest => dest.Apprenticeships, opt => opt.MapFrom(src => src.Apprenticeships));

            CreateMap<Apprenticeship, ApprenticeshipViewModel>()
                .ForMember(dest => dest.ApprenticeshipFullName, opt => opt.MapFrom(src => string.Format("{0} {1}",
                                       src.FirstName, src.LastName)))
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore());

            CreateMap<TrainingProvider, TrainingProviderViewModel>();
                
        }
    }
}