using AutoMapper;
using DasEnglishFraction = SFA.DAS.EmployerFinance.Models.Levy.DasEnglishFraction;
using ApiTypesDasEnglishFraction = SFA.DAS.EmployerFinance.Api.Types.DasEnglishFraction;

namespace SFA.DAS.EmployerFinance.Api.Mappings
{
    public class LevyMappings : Profile
    {
        public LevyMappings()
        {
            CreateMap<DasEnglishFraction, ApiTypesDasEnglishFraction>();
        }
    }
}