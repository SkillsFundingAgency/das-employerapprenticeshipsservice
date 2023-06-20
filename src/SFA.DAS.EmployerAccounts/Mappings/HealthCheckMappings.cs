using AutoMapper;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class HealthCheckMappings : Profile
{
    public HealthCheckMappings()
    {
        CreateMap<HealthCheck, HealthCheckDto>();
    }
}