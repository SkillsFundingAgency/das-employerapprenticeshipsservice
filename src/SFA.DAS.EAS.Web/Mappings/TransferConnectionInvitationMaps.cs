using AutoMapper;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;
using SFA.DAS.EAS.Web.ViewModels.Transfers;

namespace SFA.DAS.EAS.Web.Mappings
{
    public class TransferConnectionInvitationMaps : Profile
    {
        public TransferConnectionInvitationMaps()
        {
            CreateMap<GetTransferConnectionInvitationAccountResponse, SendTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetSentTransferConnectionInvitationResponse, SentTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());
            
            CreateMap<GetTransferConnectionInvitationsResponse, TransferConnectionInvitationsViewModel>();
        }
    }
}