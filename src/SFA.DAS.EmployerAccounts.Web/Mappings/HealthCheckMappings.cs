using AutoMapper;
using SFA.DAS.EmployerAccounts.Queries.GetHealthCheck;

namespace SFA.DAS.EmployerAccounts.Web.Mappings;

public class HealthCheckMappings : Profile
{
    public HealthCheckMappings()
    {
        CreateMap<GetHealthCheckQueryResponse, HealthCheckViewModel>();
    }
}