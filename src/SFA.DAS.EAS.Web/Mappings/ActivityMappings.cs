using AutoMapper;
using SFA.DAS.EAS.Application.Queries.GetActivities;
using SFA.DAS.EAS.Application.Queries.GetLatestActivities;
using SFA.DAS.EAS.Web.ViewModels.Activities;

namespace SFA.DAS.EAS.Web.Mappings
{
    public class ActivityMappings : Profile
    {
        public ActivityMappings()
        {
            CreateMap<GetActivitiesResponse, ActivitiesViewModel>();
            CreateMap<GetLatestActivitiesResponse, LatestActivitiesViewModel>();
        }
    }
}