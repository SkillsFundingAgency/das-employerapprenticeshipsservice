using AutoMapper;
using SFA.DAS.EmployerFinance.Commands.ApproveTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Commands.RejectTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Mappings;
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
            CreateMap<GetTransferTransactionDetailsResponse, TransferTransactionDetailsViewModel>();

            CreateMap<GetApprovedTransferConnectionInvitationResponse, ApprovedTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetReceivedTransferConnectionInvitationResponse, ApproveTransferConnectionInvitationCommand>();

            CreateMap<GetReceivedTransferConnectionInvitationResponse, ReceiveTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.TransferConnectionInvitationId, o => o.MapFrom(r => r.TransferConnectionInvitation.Id));

            CreateMap<GetReceivedTransferConnectionInvitationResponse, RejectTransferConnectionInvitationCommand>();

            CreateMap<GetRejectedTransferConnectionInvitationResponse, DeleteTransferConnectionInvitationCommand>();

            CreateMap<GetRejectedTransferConnectionInvitationResponse, RejectedTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.TransferConnectionInvitationId, o => o.MapFrom(r => r.TransferConnectionInvitation.Id));

            CreateMap<GetSentTransferConnectionInvitationResponse, SentTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetTransferAllowanceResponse, TransferAllowanceViewModel>()
                .ForMember(m => m.RemainingTransferAllowance, opt => opt.MapFrom(src => src.TransferAllowance.RemainingTransferAllowance))
                .ForMember(m => m.StartingTransferAllowance, opt => opt.MapFrom(src => src.TransferAllowance.StartingTransferAllowance));
            
            CreateMap<GetTransferConnectionInvitationAuthorizationResponse, TransferConnectionInvitationAuthorizationViewModel>();
            
            CreateMap<GetTransferConnectionInvitationResponse, DeleteTransferConnectionInvitationCommand>();

            CreateMap<GetTransferConnectionInvitationResponse, TransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.TransferConnectionInvitationId, o => o.MapFrom(r => r.TransferConnectionInvitation.Id));

            CreateMap<GetTransferConnectionInvitationsResponse, TransferConnectionInvitationsViewModel>();

            CreateMap<GetTransferRequestsResponse, TransferRequestsViewModel>();
            
            CreateMap<SendTransferConnectionInvitationResponse, SendTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.ReceiverAccountPublicHashedId, o => o.ResolveUsing<PublicHashedResolver, long>(i => i.ReceiverAccount.Id));
        }
    }
}

