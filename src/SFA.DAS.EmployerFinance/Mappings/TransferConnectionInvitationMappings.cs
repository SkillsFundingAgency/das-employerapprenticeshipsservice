using System.Linq;
using AutoMapper;
using SFA.DAS.EmployerFinance.Api.Types;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Mappings
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
                .ForMember(m => m.FundingEmployerHashedAccountId, o => o.ResolveUsing<HashedResolver, long>(i => i.SenderAccount.Id))
                .ForMember(m => m.FundingEmployerPublicHashedAccountId, o => o.ResolveUsing<PublicHashedResolver, long>(i => i.SenderAccount.Id))
                .ForMember(m => m.FundingEmployerAccountName, o => o.MapFrom(i => i.SenderAccount.Name));
        }
    }
}
