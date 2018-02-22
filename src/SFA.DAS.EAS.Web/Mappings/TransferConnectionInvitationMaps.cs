using AutoMapper;
using SFA.DAS.EAS.Application.Commands.ApproveTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Commands.RejectTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetApprovedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetRejectedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation;
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
            CreateMap<GetApprovedTransferConnectionInvitationResponse, ApprovedTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetReceivedTransferConnectionInvitationResponse, ReceiveTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.ApproveTransferConnectionInvitationCommand, o => o.Ignore())
                .ForMember(m => m.RejectTransferConnectionInvitationCommand, o => o.Ignore());

            CreateMap<GetRejectedTransferConnectionInvitationResponse, RejectedTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetSentTransferConnectionInvitationResponse, SentTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetTransferConnectionInvitationAccountResponse, SendTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.SendTransferConnectionInvitationCommand, o => o.Ignore());

            CreateMap<GetTransferConnectionInvitationResponse, TransferConnectionInvitationViewModel>();
            CreateMap<GetTransferConnectionInvitationsResponse, TransferConnectionInvitationsViewModel>();
            CreateMap<ReceiveTransferConnectionInvitationViewModel, ApproveTransferConnectionInvitationCommand>();
            CreateMap<ReceiveTransferConnectionInvitationViewModel, RejectTransferConnectionInvitationCommand>();
            CreateMap<SendTransferConnectionInvitationViewModel, SendTransferConnectionInvitationCommand>();

            CreateMap<StartTransferConnectionInvitationViewModel, GetTransferConnectionInvitationAccountQuery>()
                .ForMember(m => m.ReceiverAccountPublicHashedId, o => o.Ignore());
        }
    }
}