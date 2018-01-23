using AutoMapper;
using SFA.DAS.EAS.Application.Queries.GetCreatedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitationQuery;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitation;

namespace SFA.DAS.EAS.Web.Mappings
{
    public class TransferConnectionMaps : Profile
    {
        public TransferConnectionMaps()
        {
            CreateMap<GetCreatedTransferConnectionInvitationResponse, CreatedTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());

            CreateMap<GetSentTransferConnectionInvitationResponse, SentTransferConnectionInvitationViewModel>()
                .ForMember(m => m.Choice, o => o.Ignore());
        }
    }
}