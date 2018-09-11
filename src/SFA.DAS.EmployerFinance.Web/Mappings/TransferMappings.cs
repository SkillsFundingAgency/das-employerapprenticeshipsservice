using AutoMapper;
using SFA.DAS.EmployerFinance.Commands.ApproveTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Commands.RejectTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetApprovedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetRejectedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetTransferAllowance;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitationAuthorization;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EmployerFinance.Queries.GetTransferRequests;
using SFA.DAS.EmployerFinance.Queries.GetTransferTransactionDetails;
using SFA.DAS.EmployerFinance.Queries.SendTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.Mappings
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
            CreateMap<GetRejectedTransferConnectionInvitationResponse, DeleteTransferConnectionInvitationCommand>();

            CreateMap<GetRejectedTransferConnectionInvitationResponse, RejectedTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.DeleteTransferConnectionInvitationCommand, o => o.MapFrom(r => r));

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
            CreateMap<GetTransferTransactionDetailsResponse, TransferTransactionDetailsViewModel>();

        }
    }
}