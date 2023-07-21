using AutoMapper;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class CohortMappings : Profile
{
    public CohortMappings()
    {
        CreateMap<CohortSummary, Cohort>()                
            .ForMember(target => target.Id, opt => opt.MapFrom(src => src.CohortId))
            .ForMember(target => target.NumberOfDraftApprentices, opt => opt.MapFrom(src => src.NumberOfDraftApprentices))
            .ForMember(target => target.Apprenticeships, opt => opt.Ignore())                
            .ForMember(target => target.HashedId, opt => opt.Ignore())
            .ForMember(target => target.TrainingProvider, opt => opt.Ignore())
            .ForMember(target => target.CohortStatus, opt => opt.MapFrom(src => GetStatus(src)));

    }

    private static CohortStatus GetStatus(CohortSummary cohort)
    {
        if (cohort.IsDraft && cohort.WithParty == Party.Employer)
            return CohortStatus.Draft;
        
        if (!cohort.IsDraft && cohort.WithParty == Party.Employer)
            return CohortStatus.Review;
        
        if (!cohort.IsDraft && cohort.WithParty == Party.Provider)
            return CohortStatus.WithTrainingProvider;
        
        if (!cohort.IsDraft && cohort.WithParty == Party.TransferSender)
            return CohortStatus.WithTransferSender;
        
        return CohortStatus.Unknown;
    }
}