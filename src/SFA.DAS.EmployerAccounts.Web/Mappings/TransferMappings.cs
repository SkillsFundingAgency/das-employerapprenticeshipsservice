﻿using AutoMapper;
using SFA.DAS.EmployerAccounts.Commands.ApproveTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Commands.RejectTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetApprovedTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetRejectedTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetTransferAllowance;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitationAuthorization;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EmployerAccounts.Queries.GetTransferRequests;
using SFA.DAS.EmployerAccounts.Queries.SendTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Mappings
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

            CreateMap<GetTransferAllowanceResponse, TransferAllowanceViewModel>()
                .ForMember(m => m.RemainingTransferAllowance, opt => opt.MapFrom(src => src.TransferAllowance.RemainingTransferAllowance))
                .ForMember(m => m.StartingTransferAllowance, opt => opt.MapFrom(src => src.TransferAllowance.StartingTransferAllowance));

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