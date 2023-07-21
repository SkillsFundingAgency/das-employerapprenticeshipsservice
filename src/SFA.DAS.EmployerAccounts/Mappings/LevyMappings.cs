using AutoMapper;
using DasEnglishFraction = SFA.DAS.EmployerAccounts.Models.Levy.DasEnglishFraction;
using OuterDasEnglishFraction = SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.Finance.DasEnglishFraction;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class LevyMappings : Profile
{
    public LevyMappings()
    {
        CreateMap<OuterDasEnglishFraction, DasEnglishFraction>();
    }
}