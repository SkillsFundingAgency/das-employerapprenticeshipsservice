using AutoMapper;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class HealthCheckMappings : Profile
    {
        public HealthCheckMappings()
        {
            CreateMap<HealthCheck, HealthCheckDto>();
        }
    }
}