using AutoMapper;
using SFA.DAS.EmployerAccounts.Models.Commitments;
using SFA.DAS.EmployerAccounts.Queries.GetAccountCohort;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Mappings
{
    public class CohortV2Mapping : Profile
    {
        public CohortV2Mapping()
        {
            CreateMap<CohortV2, CohortV2ViewModel>()
               .ForMember(dest => dest.HashedCohortReference, opt => opt.Ignore())
               .ForMember(dest => dest.HashedDraftApprenticeshipId, opt => opt.Ignore());               

            CreateMap<GetAccountCohortResponse, CohortV2ViewModel>()
                .ForMember(dest => dest.CohortId, opt => opt.Ignore())
                .ForMember(dest => dest.NumberOfDraftApprentices, opt => opt.Ignore())
                .ForMember(dest => dest.CohortStatus, opt => opt.Ignore())
                .ForMember(dest => dest.Apprenticeships, opt => opt.Ignore());

            CreateMap<Apprenticeship, ApprenticeshipViewModel>()
                .ForMember(dest => dest.ApprenticeshipFullName, opt => opt.MapFrom(src => string.Format("{0} {1}",
                                       src.FirstName, src.LastName)))
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore());

            CreateMap<TrainingProvider, TrainingProviderViewModel>();
                
        }
    }
}