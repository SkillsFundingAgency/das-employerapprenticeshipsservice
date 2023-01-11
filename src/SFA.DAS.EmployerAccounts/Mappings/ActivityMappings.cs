using AutoMapper;
using SFA.DAS.EmployerAccounts.Models.Activities;
using SFA.DAS.EmployerAccounts.Queries.GetActivities;
using SFA.DAS.EmployerAccounts.Queries.GetLatestActivities;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class ActivityMappings : Profile
{
    public ActivityMappings()
    {
        CreateMap<GetActivitiesResponse, ActivitiesViewModel>();
        CreateMap<GetLatestActivitiesResponse, LatestActivitiesViewModel>();
    }
}