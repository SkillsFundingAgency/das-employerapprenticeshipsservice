using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Mappings;

public class TransferConnectionInvitationMappings : Profile
{
    public TransferConnectionInvitationMappings()
    {
        CreateMap<TransferConnectionInvitation, TransferConnectionViewModel>()
            .ForMember(m => m.FundingEmployerAccountId, o => o.MapFrom(i => i.SenderAccount.Id))
            .ForMember(m => m.FundingEmployerHashedAccountId, o => o.MapFrom(i => i.SenderAccount.HashedId))
            .ForMember(m => m.FundingEmployerPublicHashedAccountId, o => o.MapFrom(i => i.SenderAccount.PublicHashedId))
            .ForMember(m => m.FundingEmployerAccountName, o => o.MapFrom(i => i.SenderAccount.Name));
    }
}