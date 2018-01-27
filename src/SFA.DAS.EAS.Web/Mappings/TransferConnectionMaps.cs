using AutoMapper;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitationQuery;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.Mappings
{
    public class TransferConnectionMaps : Profile
    {
        public TransferConnectionMaps()
        {
            CreateMap<GetTransferConnectionInvitationAccountResponse, SendTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetSentTransferConnectionInvitationResponse, SentTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());
        }
    }
}