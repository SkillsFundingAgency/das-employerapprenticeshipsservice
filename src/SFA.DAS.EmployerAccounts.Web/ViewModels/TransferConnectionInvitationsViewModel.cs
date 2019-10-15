using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class TransferConnectionInvitationsViewModel : IAuthorizationContextModel
    {
        [IgnoreMap]
        public long AccountId { get; set; }

        public IEnumerable<TransferConnectionInvitationDto> TransferConnectionInvitations { get; set; }
    }
}