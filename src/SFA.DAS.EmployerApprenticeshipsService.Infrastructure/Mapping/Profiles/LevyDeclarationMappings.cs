using AutoMapper;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EAS.Infrastructure.Mapping.Profiles
{
    public class LevyDeclarationMappings : Profile
    {
        public LevyDeclarationMappings()
        {
            CreateMap<LevyDeclarationView, LevySchemeDeclarationUpdatedMessage>();
        }
    }
}