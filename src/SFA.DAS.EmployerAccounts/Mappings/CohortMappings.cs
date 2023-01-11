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

    private CohortStatus GetStatus(CohortSummary cohort)
    {
        if (cohort.IsDraft && cohort.WithParty == Party.Employer)
            return CohortStatus.Draft;
        else if (!cohort.IsDraft && cohort.WithParty == Party.Employer)
            return CohortStatus.Review;
        else if (!cohort.IsDraft && cohort.WithParty == Party.Provider)
            return CohortStatus.WithTrainingProvider;
        else if (!cohort.IsDraft && cohort.WithParty == Party.TransferSender)
            return CohortStatus.WithTransferSender;
        else
            return CohortStatus.Unknown;
    }
}