using AutoMapper;
using SFA.DAS.EAS.Application.Queries.GetActivities;
using SFA.DAS.EAS.Web.ViewModels.Activities;

namespace SFA.DAS.EAS.Web.Mappings
{
    public class ActivityMaps : Profile
    {
        public ActivityMaps()
        {
            CreateMap<GetActivitiesResponse, ActivitiesViewModel>();
        }
    }
}