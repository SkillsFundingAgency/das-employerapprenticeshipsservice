using AutoMapper;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Recruit;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class VacancyMappings : Profile
{
    public VacancyMappings()
    {
        CreateMap<VacancySummary, Vacancy>()
            .ForMember(m => m.ManageVacancyUrl, o => o.MapFrom(f => f.RaaManageVacancyUrl))
            .ForMember(m => m.Status, o => o.MapFrom(f => Enum.Parse(typeof(VacancyStatus), f.Status, true)));
    }
}