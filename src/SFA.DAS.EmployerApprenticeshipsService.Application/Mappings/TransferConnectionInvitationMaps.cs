using System.Linq;
using AutoMapper;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class TransferConnectionInvitationMaps : Profile
    {
        public TransferConnectionInvitationMaps()
        {
            CreateMap<TransferConnectionInvitation, TransferConnectionInvitationDto>()
                .ForMember(m => m.Changes, o => o.MapFrom(i => i.Changes.OrderBy(c => c.CreatedDate)));

            CreateMap<TransferConnectionInvitationChange, TransferConnectionInvitationChangeDto>();
        }
    }
}