using AutoMapper;
using SFA.DAS.EmployerAccounts.Models.Recruit;

namespace SFA.DAS.EmployerAccounts.Web.Mappings;

public class VacancyMappings : Profile
{
    public VacancyMappings()
    {
        CreateMap<Vacancy, VacancyViewModel>()
            .ForMember(m => m.ClosingDateText, o => o.MapFrom(r => r.ClosingDate.HasValue ? r.ClosingDate.Value.ToString("dd MMMM yyyy") : ""))
            .ForMember(m => m.ClosedDateText, o => o.MapFrom(r => r.ClosedDate.HasValue ? r.ClosedDate.Value.ToString("dd MMMM yyyy") : r.ClosingDate.HasValue ? r.ClosingDate.Value.ToString("dd MMMM yyyy") : ""));
    }
}