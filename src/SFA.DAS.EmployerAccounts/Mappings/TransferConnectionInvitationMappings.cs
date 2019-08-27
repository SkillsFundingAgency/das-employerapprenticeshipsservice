using System.Linq;
using AutoMapper;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;

namespace SFA.DAS.EmployerAccounts.Mappings
{
    public class TransferConnectionInvitationMappings : Profile
    {
        public TransferConnectionInvitationMappings()
        {
            CreateMap<TransferConnectionInvitation, TransferConnectionInvitationDto>()
                .ForMember(m => m.Changes, o => o.MapFrom(i => i.Changes.OrderBy(c => c.CreatedDate)));

            CreateMap<TransferConnectionInvitationChange, TransferConnectionInvitationChangeDto>();

            CreateMap<TransferConnectionInvitation, TransferConnection>()
                .ForMember(m => m.FundingEmployerAccountId, o => o.MapFrom(i => i.SenderAccount.Id))
                .ForMember(m => m.FundingEmployerHashedAccountId, o => o.MapFrom(i => i.SenderAccount.HashedId))
                .ForMember(m => m.FundingEmployerPublicHashedAccountId, o => o.MapFrom(i => i.SenderAccount.PublicHashedId))
                .ForMember(m => m.FundingEmployerAccountName, o => o.MapFrom(i => i.SenderAccount.Name));
        }
    }
}