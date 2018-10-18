using AutoMapper;
using SFA.DAS.EmployerFinance.Queries.GetHealthCheck;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.Mappings
{
    public class HealthCheckMappings : Profile
    {
        public HealthCheckMappings()
        {
            CreateMap<GetHealthCheckQueryResponse, HealthCheckViewModel>();
        }
    }
}