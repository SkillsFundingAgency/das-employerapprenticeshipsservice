using AutoMapper;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class TransferConnectionInvitationMaps : Profile
    {
        public TransferConnectionInvitationMaps()
        {
            CreateMap<TransferConnectionInvitation, TransferConnectionInvitationDto>();
        }
    }
}