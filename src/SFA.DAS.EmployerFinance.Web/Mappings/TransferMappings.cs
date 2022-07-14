using AutoMapper;
using SFA.DAS.EmployerFinance.Mappings;
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

            CreateMap<SendTransferConnectionInvitationResponse, SendTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore())
                .ForMember(m => m.ReceiverAccountPublicHashedId, o => o.ResolveUsing<PublicHashedResolver, long>(i => i.ReceiverAccount.Id));
        }
    }
}

