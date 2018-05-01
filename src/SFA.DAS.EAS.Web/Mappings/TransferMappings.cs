using AutoMapper;
using SFA.DAS.EAS.Application.Commands.ApproveTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Commands.RejectTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetApprovedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetRejectedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAuthorization;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EAS.Application.Queries.GetTransferRequests;
using SFA.DAS.EAS.Application.Queries.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;
using SFA.DAS.EAS.Web.ViewModels.Transfers;

namespace SFA.DAS.EAS.Web.Mappings
{
    public class TransferMappings : Profile
    {
        public TransferMappings()
        {
            CreateMap<GetApprovedTransferConnectionInvitationResponse, ApprovedTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetReceivedTransferConnectionInvitationResponse, ApproveTransferConnectionInvitationCommand>();

            CreateMap<GetReceivedTransferConnectionInvitationResponse, ReceiveTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.ApproveTransferConnectionInvitationCommand, o => o.MapFrom(r => r))
                .ForMember(m => m.RejectTransferConnectionInvitationCommand, o => o.MapFrom(r => r));

            CreateMap<GetReceivedTransferConnectionInvitationResponse, RejectTransferConnectionInvitationCommand>();

            CreateMap<GetRejectedTransferConnectionInvitationResponse, RejectedTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetSentTransferConnectionInvitationResponse, SentTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetTransferAllowanceResponse, TransferAllowanceViewModel>();
            CreateMap<GetTransferConnectionInvitationAuthorizationResponse, TransferConnectionInvitationAuthorizationViewModel>();
            CreateMap<GetTransferConnectionInvitationResponse, DeleteTransferConnectionInvitationCommand>();

            CreateMap<GetTransferConnectionInvitationResponse, TransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.DeleteTransferConnectionInvitationCommand, o => o.MapFrom(r => r));
            
            CreateMap<GetTransferConnectionInvitationsResponse, TransferConnectionInvitationsViewModel>();
            CreateMap<GetTransferRequestsResponse, TransferRequestsViewModel>();
            CreateMap<SendTransferConnectionInvitationResponse, SendTransferConnectionInvitationCommand>();

            CreateMap<SendTransferConnectionInvitationResponse, SendTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.SendTransferConnectionInvitationCommand, o => o.MapFrom(r => r));
        }
    }
}